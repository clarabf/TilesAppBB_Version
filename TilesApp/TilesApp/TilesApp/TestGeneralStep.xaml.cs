using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TilesApp.Models;
using Xamarin.Forms;

namespace TilesApp
{
    public partial class TestGeneralStep : ContentPage
    {

        Tile tile;
        int task_id;
        int max_steps;
        int current_step;
        string worker;
        string pdf;

        public TestGeneralStep()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

        }

        public TestGeneralStep(Tile t, int t_id, int m_steps, int c_step, string wor, string url, int s_order)
        {
            InitializeComponent();
            tile = t;
            task_id = t_id;
            max_steps = m_steps;
            current_step = c_step;
            worker = wor;
            pdf = url;
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    pdfViewer.Source = new UrlWebViewSource() { Url = "http://drive.google.com/viewerng/viewer?embedded=true&url=" + url };
            //});
            NavigationPage.SetHasNavigationBar(this, false);
            
        }

        private async void GoToNextStep( object sender, EventArgs args)
        {
            Button b = (Button)sender;
            int status;

            if (b.Text == "SKIP STEP") status = 2;
            else status = 3;
            Console.WriteLine(status);

            HttpClient client = new HttpClient();

            try
            {
                // Update task information
                var dict = new Dictionary<string, object>();
                dict.Add("task_id", task_id);
                dict.Add("worker", worker);
                dict.Add("current_status", status);
                var content = new StringContent(JsonConvert.SerializeObject(dict), Encoding.UTF8, "application/json");
                var response = await client.PutAsync("https://blackboxerpapi.azurewebsites.net/api/SetTaskStatus/", content);
                var successS = await response.Content.ReadAsStringAsync();
                bool success = bool.Parse(successS);

                response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetNextTask?tile_id=" + tile.id);
                var taskS = await response.Content.ReadAsStringAsync();
                TileTask new_task = JsonConvert.DeserializeObject<TileTask>(taskS);

                response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetStep?step_id=" + new_task.step_id);
                var stepS = await response.Content.ReadAsStringAsync();
                Step next_step = JsonConvert.DeserializeObject<Step>(stepS);

                int next_step_order = next_step.step_order;
                string next_step_url = next_step.url;

                if (next_step_order == max_steps)
                {
                    response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetSkippedTasks?tile_id=" + tile.id);
                    var skippedS = await response.Content.ReadAsStringAsync();
                    List<TileTask> listSkipped = JsonConvert.DeserializeObject<List<TileTask>>(skippedS);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new TestLastStep(listSkipped, tile, new_task.id, max_steps, worker, next_step_url, next_step_order));
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new TestGeneralStep(tile, new_task.id, max_steps, next_step_order, worker, next_step_url, next_step_order));
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
