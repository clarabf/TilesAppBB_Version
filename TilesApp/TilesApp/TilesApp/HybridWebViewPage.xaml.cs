using Xamarin.Forms;

namespace TilesApp
{
	public partial class HybridWebViewPage : ContentPage
	{
		public HybridWebViewPage (string qrScanned)
		{
			InitializeComponent ();
            (Application.Current as App).qrScanned = qrScanned;
            hybridWebView.RegisterActionS(data => csharpFunction(data));
        }

        private void csharpFunction(string data)
        {
            if (data!="")
            {
                DisplayAlert("Alert", "Hello " + data + "!", "OK");
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new ScanView((Application.Current as App).webView));
                });
            }
        }
    }
}