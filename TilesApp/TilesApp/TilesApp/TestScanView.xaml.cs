using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using ZXing;
using Newtonsoft.Json;
using System.Text;

namespace TilesApp
{
    public partial class TestScanView : ContentPage
    {

        private class DataReceived
        {
            public int Laststep { get; set; }    
            public int Maxsteps { get; set; }      
            public int Category { get; set; }
            public string Pdf { get; set; }
        }

        public TestScanView(int tile_id)
        {
            InitializeComponent();
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
            DataReceived tileInformation = null;

            try
            {
               //SetFrameCode(tile_id, code);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                // JS function to load page in the correct step
                Navigation.PopModalAsync(true);
                Navigation.PushModalAsync(new TestTiles());
            });
        }
    }
}

