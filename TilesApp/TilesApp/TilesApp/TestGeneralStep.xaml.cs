using System;
using Xamarin.Forms;

namespace TilesApp
{
    public partial class TestGeneralStep : ContentPage
    {

        int tile_id;
        int task_id;
        int max_steps;
        int current_step;
        string worker;
        string pdf;

        public TestGeneralStep(int tile, int t_id, int m_steps, int c_step, string wor, string url)
        {
            InitializeComponent();
            tile_id = tile;
            task_id = t_id;
            max_steps = m_steps;
            current_step = c_step;
            worker = wor;
            pdf = url;
            pdfLabel.Text = "Task: " + task_id + "\nWorker: " + worker + "\nMaxSteps: " + max_steps + "\nStep: " + current_step + "\nUrl: " + pdf;
            NavigationPage.SetHasNavigationBar(this, false);
            
        }

        private async void GoToNextStep( object sender, EventArgs args)
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
