﻿using Xamarin.Forms;

namespace TilesApp
{
	public partial class HybridWebViewPage : ContentPage
	{
		public HybridWebViewPage ()
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            //Register the actions for the JavascriptWebViewClient
            hybridWebView.RegisterActionS(data => showAlert(data));
            hybridWebView.RegisterActionV(scanQR);
        }

        private void showAlert(string data)
        {
            DisplayAlert("Alert", "Hello " + data + "!", "OK");
        }

        private void scanQR()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PopModalAsync(true);
                Navigation.PushModalAsync(new ScanView((Application.Current as App).webView));
            });
        }
    }
}