using Android.Webkit;
using Xamarin.Forms;

namespace TilesApp.Droid
{
    public class JavascriptWebViewClient : WebViewClient
    {
        string _qr;

        public JavascriptWebViewClient(string qr)
        {
            _qr = qr;
        }

        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            base.OnPageFinished(view, url);
            (Application.Current as App).webView = view;
            view.EvaluateJavascript("javascript:function invokeCSharpAction(data){jsBridge.invokeAction(data);}", null);
            view.EvaluateJavascript("javascript:function invokeVoidCSharpAction(){jsBridge.invokeVoidAction();}", null);
        }
    }
}