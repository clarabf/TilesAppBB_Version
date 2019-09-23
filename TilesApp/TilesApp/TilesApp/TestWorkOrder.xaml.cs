using System;
using Xamarin.Forms;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
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
            else if (b.Text == "Work Order 2") wo = 2;
            else if (b.Text == "Work Order 3") wo = 3;
            else if (b.Text == "Work Order 4") wo = 4;
            else if (b.Text == "Work Order 5") wo = 5;
            else wo = 6;


            HttpClient client = new HttpClient();

            try
            {
                var response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetTilesOfWorkOrder?work_order_id" + wo);
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
    }
}
