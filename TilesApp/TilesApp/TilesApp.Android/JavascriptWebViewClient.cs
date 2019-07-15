using Android.Webkit;
using Xamarin.Forms;

namespace TilesApp.Droid
{
    public class JavascriptWebViewClient : WebViewClient
    {
        string _javascript;
        string _qr;

        public JavascriptWebViewClient(string javascript, string qr)
        {
            _javascript = javascript;
            _qr = qr;
        }

        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            base.OnPageFinished(view, url);
            (Application.Current as App).webView = view;
            view.EvaluateJavascript(_javascript, null);
        }
    }
}