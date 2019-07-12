using Xamarin.Forms;

namespace TilesApp
{
	public partial class HybridWebViewPage : ContentPage
	{
		public HybridWebViewPage (string qrScanned)
		{
			InitializeComponent ();
            (Application.Current as App).qrScanned = qrScanned;
            hybridWebView.RegisterActionS(data => test(data));
        }

        private void test(string data)
        {
            DisplayAlert("Alert", "QR: " + data, "OK");
            
        }
    }
}