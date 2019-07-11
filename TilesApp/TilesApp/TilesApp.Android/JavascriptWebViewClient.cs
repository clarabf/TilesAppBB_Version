using Android.Webkit;

namespace TilesApp.Droid
{
    public class JavascriptWebViewClient : WebViewClient
    {
        string _javascript;

        public JavascriptWebViewClient(string javascript)
        {
            _javascript = javascript;
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            view.EvaluateJavascript(_javascript, null);
            string command = "test(19);";
            view.EvaluateJavascript(command, null);
        }
    }
}