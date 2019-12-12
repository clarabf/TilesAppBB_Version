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

            //testing SACO app
            MainPage = new NavigationPage(new Rfid.Views.MainPage());

            //string json = jsParser.GenerateJSON();
            //MainPage = new NavigationPage(new JSONPage(json));

            //MainPage = new NavigationPage(new JSONPageInit());
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