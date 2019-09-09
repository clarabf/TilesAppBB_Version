using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using ZXing;
using Newtonsoft.Json;
using System.Text;

namespace TilesApp
{
    public partial class ScanView : ContentPage
    {
        Android.Webkit.WebView mainPage;

        private class DataReceived
        {
            public int Laststep { get; set; }    
            public int Maxsteps { get; set; }      
            public int Category { get; set; }
            public string Pdf { get; set; }
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

            // We scan the tile id
            string qrScanned = result.Text.ToString();

            HttpClient client = new HttpClient();
            DataReceived tileInformation = null;

            try
            {
                // Getting tile information
                var dict = new Dictionary<string, int>();
                dict.Add("id", int.Parse(qrScanned));
                var content = new StringContent(JsonConvert.SerializeObject(dict), Encoding.UTF8, "application/json");
                var postResponse = await client.PostAsync("https://blackboxerpapi.azurewebsites.net/api/GetTile/", content);
                var tile_info = await postResponse.Content.ReadAsStringAsync();

                string dataTileJs;

                //var dataTile = new Dictionary<string, object>();
                //dataTile.Add("id", 0);
                //dataTile.Add("work_order_id", 1);
                //dataTile.Add("tile_type", 12);
                //dataTile.Add("frame_code", "123abc");
                //content = new StringContent(JsonConvert.SerializeObject(dataTile), Encoding.UTF8, "application/json");
                //postResponse = await client.PostAsync("https://blackboxerpapi.azurewebsites.net/api/SetTile/", content);
                //var id_tile = await postResponse.Content.ReadAsStringAsync();

                // For testing, we assume that tiles id is a number (1-9)
                if (qrScanned.Length == 1)
                {
                    tileInformation = JsonConvert.DeserializeObject<DataReceived>(tile_info);

                    // Getting the pdf url associated to the tile category
                    dict = new Dictionary<string, int>();
                    dict.Add("tile", tileInformation.Category);
                    content = new StringContent(JsonConvert.SerializeObject(dict), Encoding.UTF8, "application/json");
                    postResponse = await client.PostAsync("https://blackboxerpapi.azurewebsites.net/api/GetPdf/", content);
                    var url = await postResponse.Content.ReadAsStringAsync();

                    // Getting the pdf url of the last step
                    string url_step = url.Replace(".pdf", "_" + tileInformation.Laststep + ".pdf");

                    // Json to send to the front
                    var jsData = new Dictionary<string, object>();
                    jsData.Add("id", qrScanned);
                    jsData.Add("url", url_step);
                    jsData.Add("category", tileInformation.Category);
                    jsData.Add("step", tileInformation.Laststep);
                    jsData.Add("maxsteps", tileInformation.Maxsteps);
                    dataTileJs = JsonConvert.SerializeObject(jsData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                // JS function to load page in the correct step
                if (qrScanned.Length == 1) mainPage.EvaluateJavascript("setStep('" + tileInformation.Laststep + "')", null);
                Navigation.PopModalAsync(true);
            });
        }
    }
}

