﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TilesApp.Models;
using Xamarin.Forms;

namespace TilesApp
{
    public partial class TestFirstStep : ContentPage
    {

        Tile tile;
        int task_id;
        int max_steps;
        string worker;
        Styles styles = new Styles();

        public TestFirstStep()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public TestFirstStep(Tile t, int t_id, int m_steps, string wor, string url, int s_order)
        {
            InitializeComponent();
            tile = t;
            task_id = t_id;
            max_steps = m_steps;
            worker = wor;
                       
            for (int i = 0; i < m_steps; i++)
            {

                Style s;
                if (i == (s_order - 1)) s = styles.selectedStyle;
                else s = styles.unselectedStyle;

                var button = new Button()
                {
                    Text = $"{i + 1}",
                    ClassId = $"{i + 1}",
                    Style = s
                };

                button.Clicked += Handle_Clicked;

                stepBar.Children.Add(button);

                if (i < m_steps - 1)
                {
                    var separatorLine = new BoxView()
                    {
                        BackgroundColor = Color.Silver,
                        HeightRequest = 1,
                        WidthRequest = 5,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };
                    stepBar.Children.Add(separatorLine);
                }
            }

            skiplabel.Text = "Step " + s_order + "/" + max_steps;
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    pdfViewer.Source = new UrlWebViewSource() { Url = "http://drive.google.com/viewerng/viewer?embedded=true&url=" + url };
            //});
            NavigationPage.SetHasNavigationBar(this, false);

        }

        private void Handle_Clicked(object sender, EventArgs e)
        {
            Button b = (Button)sender;
                        
            ////////// TEST
            Tile t = new Tile();
            t.id = 2;
            t.tile_type = 3;
            b.Style = styles.selectedStyle;
            string next_step_url = "http://oboria.net/docs/pdf/ftp/3/" + b.Text + ".PDF";

            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PopModalAsync(true);
                Navigation.PushModalAsync(new TestGeneralStep(t, task_id, max_steps, "cbonillo", next_step_url, int.Parse(b.Text)));
            });
        }

        private async void GoToScan(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new TestScanView(tile));
        }

        private async void GoToNextStep( object sender, EventArgs args)
        {

            HttpClient client = new HttpClient();

            try
            {
                // Update task information
                var dict = new Dictionary<string, object>();
                dict.Add("task_id", task_id);
                dict.Add("worker", worker);
                dict.Add("current_status", 3);
                var content = new StringContent(JsonConvert.SerializeObject(dict), Encoding.UTF8, "application/json");
                var response = await client.PutAsync("https://blackboxerpapi.azurewebsites.net/api/SetTaskStatus/", content);
                var successS = await response.Content.ReadAsStringAsync();
                bool success = bool.Parse(successS);

                response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetNextTask?tile_id=" + tile.id);
                var taskS = await response.Content.ReadAsStringAsync();
                TileTask new_task = JsonConvert.DeserializeObject<TileTask>(taskS);

                int next_step_order;
                int new_task_id;
                string next_step_url;

                if (new_task != null)
                {
                    response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetStep?step_id=" + new_task.step_id);
                    var stepS = await response.Content.ReadAsStringAsync();
                    Step next_step = JsonConvert.DeserializeObject<Step>(stepS);

                    next_step_order = next_step.step_order;
                    next_step_url = next_step.url;
                    new_task_id = new_task.id;
                }
                else
                {
                    next_step_order = max_steps;
                    next_step_url = "http://oboria.net/docs/pdf/ftp/2/5.PDF";
                    new_task_id = 1;
                }

                if (next_step_order == max_steps)
                {
                    response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetSkippedTasks?tile_id=" + tile.id);
                    var skippedS = await response.Content.ReadAsStringAsync();
                    List<TileTask> listSkipped = JsonConvert.DeserializeObject<List<TileTask>>(skippedS);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new TestLastStep(listSkipped, tile, new_task_id, max_steps, worker, next_step_url, next_step_order));
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new TestGeneralStep(tile, new_task_id, max_steps, worker, next_step_url, next_step_order));
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
