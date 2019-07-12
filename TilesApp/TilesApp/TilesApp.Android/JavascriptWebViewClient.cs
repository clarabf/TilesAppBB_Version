using Android.Webkit;

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

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            view.EvaluateJavascript(_javascript, null);
            string command = "test('" + _qr + "')";
            view.EvaluateJavascript(command, null);
        }
    }
}