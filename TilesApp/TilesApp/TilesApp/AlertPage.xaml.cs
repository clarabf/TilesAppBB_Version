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
        private void btnPopupButton_Clicked(object sender, EventArgs e)
        {
            WarningView.IsVisible = true;
            activityIndicator.IsRunning = true;
        }
        private void btnPopupButton2_Clicked(object sender, EventArgs e)
        {
            SCANNEDView.IsVisible = true;
            activityIndicator.IsRunning = true;
        }
        private void btnPopupButton3_Clicked(object sender, EventArgs e)
        {
            CONTINUATIONView.IsVisible = true;
            activityIndicator.IsRunning = true;
        }
        private void btnPopupButton4_Clicked(object sender, EventArgs e)
        {
            WRONGView.IsVisible = true;
            activityIndicator.IsRunning = true;
        }
        private void btnPopupButton5_Clicked(object sender, EventArgs e)
        {
            PAUSEView.IsVisible = true;
            activityIndicator.IsRunning = true;
        }
        private void btnPopupButton6_Clicked(object sender, EventArgs e)
        {
            COMPLETEDView.IsVisible = true;
            activityIndicator.IsRunning = true;
        }
        private void btnPopupButton7_Clicked(object sender, EventArgs e)
        {
            LOGOUTView.IsVisible = true;
            activityIndicator.IsRunning = true;
        }
      

    }
}