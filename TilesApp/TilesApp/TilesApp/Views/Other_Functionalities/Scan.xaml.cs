using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using ZXing;
using Newtonsoft.Json;
using System.Text;
using TilesApp.Services;
using TilesApp.Models;

namespace TilesApp.Views
{
    public partial class Scan : ContentPage
    {

        int scanType;

        public Scan(string Content, int type)
        {
            scanType = type;

            var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
            options.TryInverted = true;
            options.TryHarder = true;
            options.AutoRotate = true;
            options.PureBarcode = false;

            InitializeComponent();
            zxing.Options = options;
            overlay.TopText = Content;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void ZXingScannerView_OnOnScanResult(Result result)
        {
            // Stop camera
            zxing.IsAnalyzing = false;
            zxing.IsScanning = false;
            
            // Reproduce sound of successful scanner
            var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Load("qrsound.mp3");
            player.Play();

            // We scan the tile id
            string qrScanned = result.Text.ToString();

            HttpClient client = new HttpClient();

            // Card employee
            switch (scanType)
            {
                case 1:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MessagingCenter.Send(this, "SetStation", qrScanned);
                        Navigation.PopModalAsync(true);
                    });
                    break;
                case 2:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new JSONPage(qrScanned));
                    });
                    break;
            }

        }

    }
}

