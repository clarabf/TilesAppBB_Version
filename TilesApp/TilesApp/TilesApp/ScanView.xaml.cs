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

            string qrScanned = result.Text.ToString();

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
                    postData.Add(new KeyValuePair<string, string>("id", "3"));
                    postData.Add(new KeyValuePair<string, string>("user", "usertest"));
                }
                
                var content = new FormUrlEncodedContent(postData);
                var postResponse = await client.PostAsync("http://oboria.net/qr/connect.php", content);
                var data = await postResponse.Content.ReadAsStringAsync();
                string url;
                string dataTileJs;
                if (qrScanned.Length == 1)
                {
                    dataReceived = JsonConvert.DeserializeObject<DataReceived>(data);
                    // If the pdf has never been consulted before, it is searched in the pdf table to store its url in the tile
                    if (dataReceived.Pdf == "")
                    {
                        postData = new List<KeyValuePair<string, string>>();
                        postData.Add(new KeyValuePair<string, string>("type", "pdf-info"));
                        postData.Add(new KeyValuePair<string, string>("id", dataReceived.Category.ToString()));
                        content = new FormUrlEncodedContent(postData);
                        postResponse = await client.PostAsync("http://oboria.net/qr/connect.php", content);
                        url = await postResponse.Content.ReadAsStringAsync();
                        postData = new List<KeyValuePair<string, string>>();
                        postData.Add(new KeyValuePair<string, string>("type", "update-url"));
                        postData.Add(new KeyValuePair<string, string>("id", qrScanned));
                        postData.Add(new KeyValuePair<string, string>("pdf", url));
                        content = new FormUrlEncodedContent(postData);
                        postResponse = await client.PostAsync("http://oboria.net/qr/connect.php", content);
                    }
                    else url = dataReceived.Pdf;

                    string url_step = url.Replace(".pdf", "_" + dataReceived.Laststep + ".pdf");
                    var jsData = new Dictionary<string, object>();
                    jsData.Add("id", qrScanned);
                    jsData.Add("url", url_step);
                    jsData.Add("category", dataReceived.Category);
                    jsData.Add("step", dataReceived.Laststep);
                    jsData.Add("maxsteps", dataReceived.Maxsteps);
                    dataTileJs = JsonConvert.SerializeObject(jsData);
                }
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

