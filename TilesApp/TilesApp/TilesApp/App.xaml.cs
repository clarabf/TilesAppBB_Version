using System.Collections.Generic;
using TilesApp.SACO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XmlRpc;
using TilesApp.Rfid.ViewModels;
using System.Linq;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TilesApp
{
    public partial class App : Application
    {
        public JSONParser jsParser = new JSONParser();

        public App()
        {

            InitializeComponent();

            // this for Progressbar steps
            //MainPage = new NavigationPage(new MainPage());

            // this for PDFViewer test
            //MainPage = new NavigationPage(new TestPDFViewer("http://docs.google.com/viewer?url=http://oboria.net/docs/pdf/ftp/Tile-420.pdf"));

            // this for the first version of the app
            //MainPage = new NavigationPage(new TestWorkOrder());

            ///////////////// this for the new version /////////////////

            //// PopUpMessage
            //MainPage = new NavigationPage(new AlertPage());

            ////testing login
            //MainPage = new NavigationPage(new Login());

            ////testing table
            //MainPage = new NavigationPage(new TableOrder("Bacall, L."));

            ////testing steps
            //Tile t = new Tile(); t.id = 2;
            //MainPage = new NavigationPage(new StepsPage(t, 2, 9, "user", "http://oboria.net/docs/pdf/ftp/6/3.PDF",3));

            //testing tke photo
            //MainPage = new NavigationPage(new SACOTakePhoto()); 

            //testing SACO app
            //OdooConnection od = new OdooConnection();
            //Dictionary<string, object> users = od.GetUsers();
            MainPage = new NavigationPage(new Rfid.Views.MainPage());

            //testing Pistol Reader
            //MainPage = new NavigationPage(new SACOReader());

            NavigationPage.SetHasNavigationBar(this, false);
        }

        /// <summary>
        /// Change the displayed tab to the one titled "Find Tag"
        /// </summary>
        public static void ShowFind()
        {
            var main = App.Current.MainPage as Rfid.Views.MainPage;
            var target = main.Children.Where(x => x.Title == "Find Tag").FirstOrDefault() as NavigationPage;
            main.CurrentPage = target;
            target.CurrentPage.Focus();
        }

        /// <summary>
        /// Change the displayed tab to the one titled "Read Write"
        /// </summary>
        public static void ShowReadWrite()
        {
           var main = App.Current.MainPage as Rfid.Views.MainPage;
            var target = main.Children.Where(x => x.Title == "Read Write").FirstOrDefault() as NavigationPage;
            main.CurrentPage = target;
            target.CurrentPage.Focus();
        }


        /// <summary>
        /// Gets the <see cref="ViewModels.ViewModelLocator"/> that will return a ViewModel for a View
        /// </summary>
        public static ViewModelLocator ViewModel { get; } = new ViewModelLocator();

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