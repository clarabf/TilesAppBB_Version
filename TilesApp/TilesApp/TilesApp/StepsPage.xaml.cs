using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TilesApp.Models;
using Xamarin.Forms;
using System.Linq;

namespace TilesApp
{
    public partial class StepsPage : ContentPage
    {

        Tile tile;
        int task_id;
        int max_steps;
        int current_step;
        string worker;
        string pdf;
        Styles styles = new Styles();
        private double width = 0;
        private double height = 0;

        public StepsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;
        }

        public StepsPage(Tile t, int t_id, int m_steps, string wor, string url, int s_order)
        {
            InitializeComponent();
            tile = t;
            task_id = t_id;
            max_steps = m_steps;
            current_step = s_order;
            worker = wor;
            pdf = url;

            for (int i = 0; i < m_steps; i++)
            {
                Style s;
                if (i == (s_order - 1)) s = styles.selectedStyle;
                else if (i < (s_order - 1)) s = styles.alreadyDoneStyle;
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
            Device.BeginInvokeOnMainThread(() =>
            {
                pdfViewer.Source = new UrlWebViewSource() { Url = "http://drive.google.com/viewerng/viewer?embedded=true&url=" + url };
            });
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
            string next_step_url = "http://oboria.net/docs/pdf/ftp/6/" + b.Text + ".PDF";
            skiplabel.Text = "Step " + b.Text + "/" + max_steps;
            pdfViewer.Source = new UrlWebViewSource() { Url = "http://drive.google.com/viewerng/viewer?embedded=true&url=" + next_step_url };

            var buttons = stepBar.Children.Where(x => x is Button).ToList();
            foreach (Button bu in buttons) {
                if (int.Parse(bu.ClassId) < int.Parse(b.ClassId)) bu.Style = styles.alreadyDoneStyle;
                else if (int.Parse(bu.ClassId) > int.Parse(b.ClassId)) bu.Style = styles.unselectedStyle;
                bu.CornerRadius = 40;
            }
        }

        private async void PausePressed( object sender, EventArgs args)
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
                //var dict = new Dictionary<string, object>();
                //dict.Add("task_id", task_id);
                //dict.Add("worker", worker);
                //dict.Add("current_status", status);
                //var content = new StringContent(JsonConvert.SerializeObject(dict), Encoding.UTF8, "application/json");
                //var response = await client.PutAsync("https://blackboxerpapi.azurewebsites.net/api/SetTaskStatus/", content);
                //var successS = await response.Content.ReadAsStringAsync();
                //bool success = bool.Parse(successS);

                //response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetNextTask?tile_id=" + tile.id);
                //var taskS = await response.Content.ReadAsStringAsync();
                //TileTask new_task = JsonConvert.DeserializeObject<TileTask>(taskS);

                //response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetStep?step_id=" + new_task.step_id);
                //var stepS = await response.Content.ReadAsStringAsync();
                //Step next_step = JsonConvert.DeserializeObject<Step>(stepS);

                //int next_step_order = next_step.step_order;
                //string next_step_url = next_step.url;

                //////// TEST
                int next_step_order = 1;

                if (next_step_order == max_steps)
                {
                    var response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetSkippedTasks?tile_id=" + tile.id);
                    var skippedS = await response.Content.ReadAsStringAsync();
                    List<TileTask> listSkipped = JsonConvert.DeserializeObject<List<TileTask>>(skippedS);
                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                    //    Navigation.PopModalAsync(true);
                    //    Navigation.PushModalAsync(new TestLastStep(listSkipped, tile, new_task.id, max_steps, worker, next_step_url, next_step_order));
                    //});
                }
                else
                {
                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                    //    Navigation.PopModalAsync(true);
                    //    Navigation.PushModalAsync(new StepsPage(tile, new_task.id, max_steps, worker, next_step_url, next_step_order));
                    //    Navigation.PushModalAsync(new AlertPage());
                    //});
                    PAUSEPopup.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async void Pause_Interrupt(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

        private void Pause_Resume(object sender, EventArgs args)
        {
            PAUSEPopup.IsVisible = false;
        }

        //protected override void OnSizeAllocated(double width, double height)
        //{
        //    base.OnSizeAllocated(width, height);
        //    if (width != this.width || height != this.height)
        //    {
        //        this.width = width;
        //        this.height = height;
        //        if (width > height)
        //        {
        //            GridStep.RowDefinitions.Clear();
        //            GridStep.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.2, GridUnitType.Star) });
        //            GridStep.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.05, GridUnitType.Star) });
        //            GridStep.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.2, GridUnitType.Star) });
        //            GridStep.RowDefinitions.Add(new RowDefinition { Height = new GridLength(7, GridUnitType.Star) });
        //        }
        //        else
        //        {
        //            GridStep.RowDefinitions.Clear();
        //            GridStep.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        //            GridStep.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.05, GridUnitType.Star) });
        //            GridStep.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.6, GridUnitType.Star) });
        //            GridStep.RowDefinitions.Add(new RowDefinition { Height = new GridLength(7, GridUnitType.Star) });

        //        }
        //    }
        //}
    }
}
