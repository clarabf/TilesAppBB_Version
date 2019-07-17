using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using ZXing;
using Newtonsoft.Json;

namespace TilesApp
{
    public partial class ScanView : ContentPage
    {

        Android.Webkit.WebView mainPage;

        private class DataReceived
        {
            public string Laststep { get; set; }    
            public int Maxsteps { get; set; }      
            public string Category { get; set; } 
        }

        public ScanView(Android.Webkit.WebView webView)
        {
            InitializeComponent();
            mainPage = webView;
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

            string qrScanned = result.Text.ToString();
            string responseString = "Error...Connect wifi for tests.";

            HttpClient client = new HttpClient();
            DataReceived dataReceived = null;

            try
            {
                var postData = new List<KeyValuePair<string, string>>();

                /////////////////////////////////////TESTS/////////////////////////////
                if (qrScanned.Length==1)
                {
                    postData.Add(new KeyValuePair<string, string>("type", "tile-info"));
                    postData.Add(new KeyValuePair<string, string>("id", qrScanned));
                }
                else
                {
                    postData.Add(new KeyValuePair<string, string>("type", "update-user"));
                    postData.Add(new KeyValuePair<string, string>("id", "4"));
                    postData.Add(new KeyValuePair<string, string>("user", "cbonillo"));
                }
                
                var content = new FormUrlEncodedContent(postData);
                var postResponse = await client.PostAsync("http://172.16.4.175/webservice/connect.php", content);
                var data = await postResponse.Content.ReadAsStringAsync();
                dataReceived = JsonConvert.DeserializeObject<DataReceived>(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                if (qrScanned.Length == 1) mainPage.EvaluateJavascript("setStep('" + dataReceived.Laststep + "')", null);
                Navigation.PopModalAsync(true);
            });
        }
    }
}

