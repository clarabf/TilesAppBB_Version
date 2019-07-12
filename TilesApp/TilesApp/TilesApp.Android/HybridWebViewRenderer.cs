using TilesApp;
using TilesApp.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace TilesApp.Droid
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Android.Webkit.WebView>
    {
        const string JavascriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";
        //const string JavascriptFunctionTest = "function testCSharp(num){jsBridge.test(num);}";
        Context _context;

        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.Cleanup();
            }
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var webView = new Android.Webkit.WebView(_context);
                    webView.Settings.JavaScriptEnabled = true;
                    webView.SetWebViewClient(new JavascriptWebViewClient($"javascript: {JavascriptFunction}", (Application.Current as App).qrScanned));
                    //webView.SetWebViewClient(new JavascriptWebViewClient($"javascript: {JavascriptFunctionTest}"));
                    SetNativeControl(webView);
                }
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                Control.LoadUrl($"file:///android_asset/Content/{Element.Uri}");
            }
        }
    }
}