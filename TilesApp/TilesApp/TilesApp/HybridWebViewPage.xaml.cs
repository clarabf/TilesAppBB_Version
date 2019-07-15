using Xamarin.Forms;

namespace TilesApp
{
	public partial class HybridWebViewPage : ContentPage
	{
		public HybridWebViewPage (string qrScanned)
		{
			InitializeComponent ();
            (Application.Current as App).qrScanned = qrScanned;
            hybridWebView.RegisterActionS(data => showDisplayAlert(data));
        }

        private void showDisplayAlert(string data)
        {
            DisplayAlert("Alert", "QR: " + data, "OK");
        }
    }
}