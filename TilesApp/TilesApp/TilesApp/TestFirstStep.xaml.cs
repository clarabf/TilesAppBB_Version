using System;
using System.Collections.Generic;
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
        string pdf;

        public TestFirstStep(Tile t, int t_id, int m_steps, string wor, string url)
        {
            InitializeComponent();
            tile = t;
            task_id = t_id;
            max_steps = m_steps;
            worker = wor;
            pdf = url;
            //pdfLabel.Text = "Task: " + task_id + "\nWorker: " + worker + "\nMaxSteps: " + max_steps + "\nUrl: " +  pdf;
            pdfViewer.Source = "http://docs.google.com/viewer?url=" + url;
            NavigationPage.SetHasNavigationBar(this, false);
            
        }

        private async void GoToScan(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new TestScanView(tile));
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
                    List<TileTask> listSkipped = new List<TileTask>();
                    next_step_url = "http://step.12.test.com/";
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new TestLastStep(listSkipped, tile.id, new_task_id, max_steps, worker, next_step_url));
                });
            }
            else {
                Device.BeginInvokeOnMainThread(() =>
                {
                    next_step_url = "http://step.7.test.com/";
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new TestGeneralStep(tile.id, new_task_id, max_steps, next_step_order, worker, next_step_url));
                });
            }
        }
    }
}
