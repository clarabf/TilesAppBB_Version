using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TilesApp
{
    public partial class TestFirstStep : ContentPage
    {

        int tile_id;
        int task_id;
        int max_steps;
        string worker;
        string pdf;

        public TestFirstStep(int tile, int t_id, int m_steps, string wor, string url)
        {
            InitializeComponent();
            tile_id = tile;
            task_id = t_id;
            max_steps = m_steps;
            worker = wor;
            pdf = url;
            pdfLabel.Text = "Task: " + task_id + "\nWorker: " + worker + "\nMaxSteps: " + max_steps + "\nUrl: " +  pdf;
            NavigationPage.SetHasNavigationBar(this, false);
            
        }

        private async void GoToScan(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new TestScanView(tile_id));
        }

        private async void GoToNextStep( object sender, EventArgs args)
        {

            // SetTaskStatus(task_id, worker,"done")
            // int new_task_id = GetNextTask(tile_id)
            // Step next_step = GetStep(step_id)
            // int next_step_order = next_step.order;
            // string next_step_url = next_step.url;

            //////////////////////////////////// TESTS /////////////////////////////////
            int next_step_order = 7;
            string next_step_url;
            int new_task_id = 2;

            if (next_step_order == max_steps)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    List<string> listSkipped = new List<string>();
                    listSkipped.Add("Task 3");
                    listSkipped.Add("Task 7");
                    listSkipped.Add("Task 11");
                    next_step_url = "http://step.12.test.com/";
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new TestLastStep(listSkipped, tile_id, new_task_id, max_steps, worker, next_step_url));
                });
            }
            else {
                Device.BeginInvokeOnMainThread(() =>
                {
                    next_step_url = "http://step.7.test.com/";
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new TestGeneralStep(tile_id, new_task_id, max_steps, next_step_order, worker, next_step_url));
                });
            }
        }
    }
}
