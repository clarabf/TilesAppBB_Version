using System;
using Xamarin.Forms;

namespace TilesApp
{
    public partial class TestFirstStep : ContentPage
    {

        int tile_id;
        int max_steps;

        public TestFirstStep(int tile, int m_steps)
        {
            InitializeComponent();
            tile_id = tile;
            max_steps = m_steps;
            NavigationPage.SetHasNavigationBar(this, false);
            
        }

        private async void GoToScan(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new TestScanView(tile_id));
        }

        private async void GoToNextStep( object sender, EventArgs args)
        {
            // GetNextTask(tile_id)
            // next_step = GetStep(step_id)
            int next_step = 7;
            if (next_step == max_steps)
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
                    //Navigation.PushModalAsync(new TestGeneralStèp(current_tile, max_steps));
                });
            }
        }
    }
}
