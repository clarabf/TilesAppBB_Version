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
using Lottie.Forms.Droid;

namespace TilesApp.Droid
{
    using Android.Bluetooth;
    using Android.Content;
    using Android.Hardware.Usb;
    using Android.Views.InputMethods;
    using Java.Lang.Reflect;
    using PCLAppConfig;
    using System.Linq;
    using System.Threading;
    using TechnologySolutions.Rfid.AsciiProtocol.Extensions;
    using TechnologySolutions.Rfid.AsciiProtocol.Transports;
    using TilesApp.Models;

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
            //ConfigurationManager.Initialise(PCLAppConfig.FileSystemStream.PortableStream.Current);
            //Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);
            LoadApplication(new App());
            monitor = new DeviceMonitor();
            ScanBluetoothDevices();
            ScanSerialDevices();            
            App.DeviceSerialNumber = Android.OS.Build.Serial;
        }
        private IAndroidLifecycle TslLifecycle
        {
            get
            {
                if (this.lifecyle == null)
                {
                    AsciiTransportsManager manager = Locator.Default.Locate<IAsciiTransportsManager>() as AsciiTransportsManager;
                    this.lifecyle = (IAndroidLifecycle)manager ?? new AndroidLifecycleNone();
                    manager.RegisterLifecycle(Locator.Default.Locate<IHostBarcodeHandler>() as HostBarcodeHandler);
                }

                return this.lifecyle;
            }
        }
        protected override void OnStart()
        {
            base.OnStart();
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
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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
                else {
                    Dictionary<string, object> InputWithDevice = new Dictionary<string, object>();
                    InputWithDevice.Add("Device", e.Device);
                    InputWithDevice.Add("Value", barcode);
                    App.Inventory.Add(InputWithDevice);
                }
            }
            return base.OnKeyDown(keyCode, e);
        }

        private string TranslateKeyCodes(string[] keyCodes)
        {
            String result = "";
            for (int i = 0; i < keyCodes.Length; i++)
            {
                if (keyCodes[i].Contains("Num"))
                {
                    result += keyCodes[i].Substring(keyCodes[i].Length - 1);
                }
                else
                if (i >= 1 && keyCodes[i - 1] == "ShiftLeft")
                {                            
                    switch (keyCodes[i])
                    {
                        case "Apostrophe":
                            result += "\"";
                            break;
                        case "LeftBracket":
                            result += "{";
                            break;
                        case "Semicolon":
                            result += ":";
                            break;
                        case "RightBracket":
                            result += "}";
                            break;
                        case "Minus":
                            result += "_";
                            break;
                        default:
                            result += keyCodes[i].ToUpper();
                            break;
                    }
                }
                else
                {
                    switch (keyCodes[i])
                    {
                        case "Slash":
                            result += "/";
                            break;
                        case "Apostrophe":
                            result += "\'";
                            break;
                        case "LeftBracket":
                            result += "[";
                            break;
                        case "Semicolon":
                            result += ";";
                            break;
                        case "RightBracket":
                            result += "]";
                            break;
                        case "Comma":
                            result += ",";
                            break;
                        case "Space":
                            result += " ";
                            break;
                        case "Minus":
                            result += "-";
                            break;
                        case "ShiftLeft":
                        case "Back":
                        case "MediaEject":                        
                            break;
                        default:
                            result += keyCodes[i].ToLower();
                            break;
                    }                    
                }
            }
            return result;
        }

        private void ScanBluetoothDevices(){
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            List<string>  ouiVendorIds = new List<string>(ConfigurationManager.AppSettings["OUI_VENDOR_IDS"].Split(new char[] { ';' }));
            List<string> ouiTransportIds = new List<string>(ConfigurationManager.AppSettings["OUI_TRANSPORTS_IDS"].Split(new char[] { ';' }));
            List<BluetoothDevice> btDevices = adapter.BondedDevices.ToList();
            for (int i = 0; i < btDevices.Count; i++)
            {
                string[] mac = btDevices[i].Address.Split(':');
                string oui = mac[0] + mac[1] + mac[2];
                if (ouiTransportIds.Contains(oui))
                {
                    continue;
                }
                else if (ouiVendorIds.Contains(oui))
                {
                    ComplexBluetoothDevice pairedDevice = new ComplexBluetoothDevice(btDevices[i], ComplexBluetoothDevice.States.Paired);
                    foreach (var compDevice in App.ViewModel.Readers.BluetoothCameraReaders.ToList())
                    {
                        if (compDevice.Device.Address == pairedDevice.Device.Address)
                        {
                            int j = App.ViewModel.Readers.BluetoothCameraReaders.IndexOf(compDevice);
                            if (j != -1)
                                App.ViewModel.Readers.BluetoothCameraReaders[j].State = pairedDevice.State;
                            break;
                        }
                    }
                    App.ViewModel.Readers.BluetoothCameraReaders.Add(pairedDevice);
                }
            }
        }

        private void ScanSerialDevices()
        {
            List<string> vendorIds = new List<string>(ConfigurationManager.AppSettings["VENDOR_IDS"].Split(new char[] { ';' }));
            List<string> productIds = new List<string>(ConfigurationManager.AppSettings["PRODUCT_IDS"].Split(new char[] { ';' }));
            manager = (UsbManager)Android.App.Application.Context.GetSystemService(Context.UsbService);
            try
            {
                device = MainActivity.device = (manager.DeviceList.Values.ToArray())[0];
                if (vendorIds.Contains(device.VendorId.ToString()) && productIds.Contains(device.ProductId.ToString()))
                {
                    foreach (var serialDevice in App.ViewModel.Readers.SerialReaders.ToList())
                    {
                        if (serialDevice.SerialNumber == device.SerialNumber)
                        {
                            return;
                        }
                    }
                    App.ViewModel.Readers.SerialReaders.Add(device);
                }
            }
            catch (Exception) {
                //MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", e.Message);
            }
        }

    }
}