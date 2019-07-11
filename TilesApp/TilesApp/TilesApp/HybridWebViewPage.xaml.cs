using System;
using Xamarin.Forms;

namespace TilesApp
{
	public partial class HybridWebViewPage : ContentPage
	{
		public HybridWebViewPage ()
		{
			InitializeComponent ();
            hybridWebView.RegisterActionS(data => test(data));
        }

        private void test(string data)
        {
            if (data=="clara") DisplayAlert("Alert", "Hello " + data, "OK");
            else DisplayAlert("Alert", "A ver qué paza aquí" + data, "OK");
        }
    }
}