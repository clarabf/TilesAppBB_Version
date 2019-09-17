using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using ZXing;
using Newtonsoft.Json;
using System.Text;
using TilesApp.Models;

namespace TilesApp
{
    public partial class TestScanView : ContentPage
    {

        Tile current_tile;

        public TestScanView(Tile t)
        {
            current_tile = t;
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
            string success = "false";
            try
            {
                var dict = new Dictionary<string, object>();
                dict.Add("id", current_tile.id);
                dict.Add("frame_code", qrScanned);
                var content = new StringContent(JsonConvert.SerializeObject(dict), Encoding.UTF8, "application/json");
                var response = await client.PutAsync("https://blackboxerpapi.azurewebsites.net/api/SetFrameCode/", content);
                success = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            if (bool.Parse(success) == true)
            {
                try
                {
                    var response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetTilesOfWorkOrder?work_order_id=" + current_tile.work_order_id);
                    var tile_info = await response.Content.ReadAsStringAsync();
                    List<Tile> listTiles = JsonConvert.DeserializeObject<List<Tile>>(tile_info);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new TestTiles(listTiles));
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopModalAsync(true);
                    DisplayAlert("Error", "Problem updating frame_code...", "Close");
                });
            }
        }
    }
}

