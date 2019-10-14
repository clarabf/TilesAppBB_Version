using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace TilesApp
{
    public partial class AlertPage : ContentPage
    {
        public AlertPage()
        {
            InitializeComponent();
        }
        private void btnPopupButton_Clicked(object sender, EventArgs e)
        {
            popupImageView.IsVisible = true;
            activityIndicator.IsRunning = true;
        }
    }
}