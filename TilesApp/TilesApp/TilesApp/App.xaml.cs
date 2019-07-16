using Android.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TilesApp
{
    public partial class App : Application
    {
        public MediaPlayer player = new MediaPlayer();
        public Android.Webkit.WebView webView;

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new HybridWebViewPage());
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
