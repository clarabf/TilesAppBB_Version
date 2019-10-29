using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using ZXing;
using Newtonsoft.Json;
using System.Text;
using TilesApp.Models;
using TilesApp.V1;

namespace TilesApp
{
    public partial class ScanQR : ContentPage
    {

        Tile current_tile;
        int scanType;

        public ScanQR(Tile t, string Content, int type)
        {
            current_tile = t;
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

        private async void ZXingScannerView_OnOnScanResult(Result result)
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
            string success = "true";

            // Card employee
            if (scanType==1)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new TableOrder(qrScanned));
                });
            }
            else
            {
                if (qrScanned=="WRONG")
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new StepsPage(current_tile, 2, 9, "wrong", "http://oboria.net/docs/pdf/ftp/6/WRONG.PDF", 4));
                    });
                }
                else
                {
                    string text;
                    if (overlay.TopText.Contains("Restart")) text = "show";
                    else text = "noShow";
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new StepsPage(current_tile, 2, 9, text, "http://oboria.net/docs/pdf/ftp/6/" + current_tile.id +".PDF", current_tile.id));
                    });
                }
            }
        }
    }
}

