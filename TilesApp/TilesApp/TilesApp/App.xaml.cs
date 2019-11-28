using Android.Content.Res;
using System;
using System.IO;
using TilesApp.ExpandableView;
using TilesApp.Models;
using TilesApp.SACO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XmlRpc;

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
            MainPage = new NavigationPage(new SACOLogin());

            //testing Pistol Reader
            //MainPage = new NavigationPage(new SACOReader());

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