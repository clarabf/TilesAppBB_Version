using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TilesApp
{
    public partial class TestLastStep : ContentPage
    {

        List<string> listSkipped = new List<string>();
        int tile_id;
        int task_id;
        int max_steps;
        string worker;
        string pdf;

        public class PickerItems
        {
            public string Name { get; set; }
        }

        public TestLastStep(List<string> l, int tile, int t_id, int m_steps, string wor, string url)
        {
            InitializeComponent();
            listSkipped = l;
            tile_id = tile;
            task_id = t_id;
            max_steps = m_steps;
            worker = wor;
            pdf = url;
            pdfLabel.Text = "Task: " + task_id + "\nWorker: " + worker + "\nMaxSteps: " + max_steps + "\nList count: " + listSkipped.Count + "\nUrl: " + pdf;

            ObservableCollection<string> objStringList = new ObservableCollection<string>();
            ObservableCollection<PickerItems> objClassList = new ObservableCollection<PickerItems>();

            if (listSkipped.Count != 0)
            {
                foreach (var s in listSkipped) objClassList.Add(new PickerItems { Name = s });
                foreach (var item in objClassList) objStringList.Add(item.Name);
                pickerDynamicData.ItemsSource = objStringList;
                finish.IsEnabled = false;
            }
            else pickerDynamicData.IsVisible = false;

            NavigationPage.SetHasNavigationBar(this, false);
            
        }

        private void PickerSelection(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;
            //put your code here
            Console.WriteLine(selectedIndex);
        }

        private async void FinishWorkOrder( object sender, EventArgs args)
        {
            Button b = (Button)sender;
            string status;

            if (b.Text == "SKIP STEP") status = "skipped";
            else status = "done";
            Console.WriteLine(status);

            // SetTaskStatus(task_id, worker,status)
            // int new_task_id = GetNextTask(tile_id)
            // Step next_step = GetStep(step_id)
            // int next_step_order = next_step.order;
            // string next_step_url = next_step.url;

            //////////////////////////////////// TESTS /////////////////////////////////
            int next_step_order = 9;
            string next_step_url = "http://step.9.test.com/";
            int new_task_id = 3;
            
            if (next_step_order == max_steps)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopModalAsync(true);
                    //Navigation.PushModalAsync(new TestLastStep(tile_id, max_steps));
                });
            }
            else {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new TestGeneralStep(tile_id, new_task_id, max_steps, next_step_order, worker, next_step_url));
                });
            }
        }
    }
}
