using System;
using Xamarin.Forms;

namespace TilesApp
{
    public partial class AlertPage : ContentPage
    {
        Boolean ActiveBopup=false;
        public AlertPage()
        {
            InitializeComponent();
        }
        private void Warning_Clicked(object sender, EventArgs e)
        {
            WARNINGView.IsVisible = true;
            //activityIndicator.IsRunning = true;
        }
        private void Scanned_Clicked(object sender, EventArgs e)
        {
            SCANNEDView.IsVisible = true;
            //activityIndicator.IsRunning = true;
        }
        private void Continuation_Clicked(object sender, EventArgs e)
        {
            CONTINUATIONView.IsVisible = true;
            //activityIndicator.IsRunning = true;
        }
        private void Wrong_Clicked(object sender, EventArgs e)
        {
            WRONGView.IsVisible = true;
            //activityIndicator.IsRunning = true;
        }
        private void Pause_Clicked(object sender, EventArgs e)
        {
            PAUSEView.IsVisible = true;
            //activityIndicator.IsRunning = true;
        }
        private void Completed_Clicked(object sender, EventArgs e)
        {
            COMPLETEDView.IsVisible = true;
            //activityIndicator.IsRunning = true;
        }
        private void Logout_Clicked(object sender, EventArgs e)
        {
            LOGOUTView.IsVisible = true;
            //activityIndicator.IsRunning = true;
        }

        private void Hide(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            switch (b.ClassId)
            {
                //used
                case "warning":
                    WARNINGView.IsVisible = false;
                    break;
                //used
                case "pause":
                    PAUSEView.IsVisible = false;
                    break;
                //used
                case "completed":
                    COMPLETEDView.IsVisible = false;
                    break;
                //used
                case "logout":
                    LOGOUTView.IsVisible = false;
                    break;
                //used
                case "wrong":
                    WRONGView.IsVisible = false;
                    break;
                //used
                case "continuation":
                    CONTINUATIONView.IsVisible = false;
                    break;
                //used
                case "scanned":
                    SCANNEDView.IsVisible = false;
                    break;
            }
        }

    }
}