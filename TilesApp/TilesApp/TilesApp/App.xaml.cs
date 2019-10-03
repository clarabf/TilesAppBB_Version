using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TilesApp
{
    public partial class App : Application
    {
        public Android.Webkit.WebView webView;

        public App()
        {
           
            InitializeComponent();
            // OLD HybridWebView
            //MainPage = new NavigationPage(new HybridWebViewPage());

            // this for Progressbar steps
            //MainPage = new NavigationPage(new MainPage()); ;

            // this for PDFViewer test
            //MainPage = new NavigationPage(new TestPDFViewer("http://docs.google.com/viewer?url=http://oboria.net/docs/pdf/ftp/Tile-420.pdf"));

            MainPage = new NavigationPage(new TestWorkOrder());
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
