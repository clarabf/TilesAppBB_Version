using System;
using Xamarin.Forms;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using TilesApp.Models;

namespace TilesApp
{
    public partial class TestWorkOrder : ContentPage
    {

        public TestWorkOrder()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void SelectWork(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            int wo;
            if (b.Text == "Work Order 1") wo = 1;
            else wo = 2;
            
            HttpClient client = new HttpClient();
            Tile tileInformation = null;

            try
            {
                var dict = new Dictionary<string, int>();
                dict.Add("work_order_id", wo);
                var content = new StringContent(JsonConvert.SerializeObject(dict), Encoding.UTF8, "application/json");
                var response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetTilesOfWorkOrder?work_order_id=1");
                //var response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetTile?id=1");
                var tile_info = await response.Content.ReadAsStringAsync();

                List<Tile> listTiles = JsonConvert.DeserializeObject<List<Tile>>(tile_info);
                //Tile newT = JsonConvert.DeserializeObject<Tile>(tile_info);

                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new TestTiles());
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
