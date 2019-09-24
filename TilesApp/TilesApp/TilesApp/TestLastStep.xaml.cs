using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using TilesApp.Models;
using Xamarin.Forms;

namespace TilesApp
{
    public partial class TestLastStep : ContentPage
    {

        List<TileTask> listSkipped = new List<TileTask>();
        Tile tile;
        int task_id;
        int max_steps;
        string worker;
        string pdf;

        public class PickerItems
        {
            public string Name { get; set; }
        }

        public TestLastStep()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }


        public TestLastStep(List<TileTask> l, Tile t, int ta_id, int m_steps, string wor, string url, int s_order)
        {
            InitializeComponent();
            listSkipped = l;
            tile = t;
            task_id = ta_id;
            max_steps = m_steps;
            worker = wor;
            pdf = url;
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    pdfViewer.Source = new UrlWebViewSource() { Url = "http://drive.google.com/viewerng/viewer?embedded=true&url=" + url };
            //});

            ObservableCollection<string> objStringList = new ObservableCollection<string>();
            ObservableCollection<PickerItems> objClassList = new ObservableCollection<PickerItems>();

            if (listSkipped.Count != 0)
            {
                foreach (var s in listSkipped) objClassList.Add(new PickerItems { Name = s.id.ToString() });
                foreach (var item in objClassList) objStringList.Add(item.Name);
                pickerDynamicData.ItemsSource = objStringList;
                finish.IsEnabled = false;
            }
            else pickerDynamicData.IsVisible = false;

            NavigationPage.SetHasNavigationBar(this, false);
            
        }

        private async void PickerSelection(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;
            string selectedItem = (string)picker.SelectedItem;
            //put your code here
            Console.WriteLine(selectedIndex);

            TileTask task = listSkipped.Find(selection => selection.id == int.Parse(selectedItem));

            HttpClient client = new System.Net.Http.HttpClient();

            try
            {
                var response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetStep?step_id=" + task.step_id);
                var stepS = await response.Content.ReadAsStringAsync();
                Step next_step = JsonConvert.DeserializeObject<Step>(stepS);

                int next_step_order = next_step.step_order;
                string next_step_url = next_step.url;

                if (next_step_order == 1)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new TestFirstStep(tile, task_id, max_steps, "cbonillo", next_step_url, next_step_order));
                    });
                }
                else if (next_step_order == max_steps)
                {
                    response = await client.GetAsync("https://blackboxerpapi.azurewebsites.net/api/GetSkippedTasks?tile_id=" + tile.id);
                    var skippedS = await response.Content.ReadAsStringAsync();
                    List<TileTask> listSkipped = JsonConvert.DeserializeObject<List<TileTask>>(skippedS);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new TestLastStep(listSkipped, tile, task_id, max_steps, "cbonillo", next_step_url, next_step_order));
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new TestGeneralStep(tile, task_id, max_steps, next_step_order, "cbonillo", next_step_url, next_step_order));
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async void FinishWorkOrder( object sender, EventArgs args)
        {

            // SetTaskStatus(task_id, worker,status)
            // int new_task_id = GetNextTask(tile_id)
            // Step next_step = GetStep(step_id)
            // int next_step_order = next_step.order;
            // string next_step_url = next_step.url;
            await Navigation.PopModalAsync(true);
           
        }
    }
}
