using Xamarin.Forms;
using ZXing;

namespace TilesApp
{
    public partial class ScanView : ContentPage
    {
        
        public ScanView()
        {
            InitializeComponent();
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
                Navigation.PopModalAsync(true);
                Navigation.PushModalAsync(new HybridWebViewPage(qrScanned));
            });

        }

    }

}

