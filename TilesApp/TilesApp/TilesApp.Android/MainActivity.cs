using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using Plugin.Media;
using System;
using System.Collections.Generic;
using TilesApp.SACO;
using Xamarin.Forms;

namespace TilesApp.Droid
{
    [Activity(Label = "TilesApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        List<string> code = new List<string>();

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
                //Print the BARCODE to the debug console
                string barcode = TranslateKeyCodes(code.ToArray());
                //System.Diagnostics.Debug.WriteLine("Barcode: " + barcode);
                //Toast.MakeText(this, "Barcode: " + barcode, ToastLength.Long).Show();
                code.Clear();
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "SendBarcode", barcode);
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
    }

}