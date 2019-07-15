using Xamarin.Forms;
using ZXing;

namespace TilesApp
{
    public partial class ScanView : ContentPage
    {

        Android.Webkit.WebView mainPage;

        public ScanView(Android.Webkit.WebView webView)
        {
            InitializeComponent();
            mainPage = webView;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void ZXingScannerView_OnOnScanResult(Result result)
        {
            // Stop camera
            zxing.IsAnalyzing = false;
            zxing.IsScanning = false;
            
            // Reproduce sound of successful scanner
            var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Load("qrsound.mp3");
            player.Play();

            string qrScanned = result.Text.ToString();

            Device.BeginInvokeOnMainThread(() =>
            {
                mainPage.EvaluateJavascript("changeTitle('" + qrScanned + "')", null);
                Navigation.PopModalAsync(true);
            });
        }
    }
}

