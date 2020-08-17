using TilesApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TilesApp.Rfid.ViewModels;
using System.Linq;
using TilesApp.Services;
using System;
using TilesApp.Models;
using Android.Bluetooth;
using Android.Hardware.Usb;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using TilesApp.Models.DataModels;
using Xamarin.Essentials;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json;
using Plugin.Toast;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TilesApp
{
    public partial class App : Application
    {
        public static string DeviceSerialNumber { get; set; }
        public static Models.Location GeoLocation { get; set; }
        public static string Station { get; set; }

        public static string CurrentProjectName { get; set; }

        public static string CurrentProjectSlug { get; set; }
        public static User User { get; set; } = new User();
        public static bool ActiveSession { get; set; } = false;
        public static bool IsConnected { get; set; } = false;

        public static List<Web_Project> Projects { get; set; }
        public static ObservableCollection<Dictionary<string, object>> Inventory { get; set; } = new ObservableCollection<Dictionary<string, object>>();
        private static LocalDatabase database;
        public static LocalDatabase Database
        {
            get
            {
                if (database == null)
                {
                 database = new LocalDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Local.db3"));
                }
                return database;
            }
        }

        private List<Web_Field> formFieldsList = new List<Web_Field>();
        public App()
        {

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjgzMzY2QDMxMzgyZTMyMmUzMEloRWJsZkxVaDlvVE1QbXk4MDVKWXlUcEh6Z3FQeUJBbDRkUURncFNrQTQ9");

            InitializeComponent();

            MainPage = new NavigationPage(new Main());
            
            ////TESTS (Careful with this test! App.IsConnect is "false" even if internet because no normal login)
            //fillTestFields();
            //MainPage = new NavigationPage(new FormPage("Visual Test", formFieldsList));
            
            NavigationPage.SetHasNavigationBar(this, false);

        }

        private void fillTestFields()
        {
            Web_Field field = new Web_Field()
            {
                Id = "1",
                Name = "rgm",
                LongName = "RegEx Multi",
                Description = "miau miau miau",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ProjectId = "testProject",
                FieldCategory = "1",
                PrimitiveType = "Chars Array (Str)",
                PrimitiveQuantity = 20,
                ValueRegEx = "^\\[(\"Option A\",?|\"Option B\",?|\"Option C\",?)*\\]$",
                Default = null,
                Created_at = "2020-07-01 12:07:20",
                Updated_at = null,
                Deleted_at = null,
            }; 
            formFieldsList.Add(field);
            field = new Web_Field()
            {
                Id = "2",
                Name = "eid",
                LongName = "Element Unique ID",
                Description = "miau miau miau",
                Slug = "eid",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ProjectId = "testProject",
                FieldCategory = "20",
                PrimitiveType = "Chars Array (Str)",
                PrimitiveQuantity = 20,
                ValueRegEx = null,
                Default = null,
                Created_at = "2020-07-01 12:07:20",
                Updated_at = null,
                Deleted_at = null,
            };
            formFieldsList.Add(field);
            field = new Web_Field()
            {
                Id = "3",
                Name = "loc",
                LongName = "Completition",
                Description = "miau miau miau",
                Slug = "loc",
                ValueIsUnique = false,
                ValueIsRequired = false,
                ProjectId = "testProject",
                FieldCategory = "1",
                PrimitiveType = "Integer",
                PrimitiveQuantity = 1,
                ValueRegEx = "^([1-9]?\\\\d|100)$",
                Default = null,
                Created_at = "2020-07-01 12:07:20",
                Updated_at = null,
                Deleted_at = null,
            };
            formFieldsList.Add(field);
            field = new Web_Field()
            {
                Id = "4",
                Name = "rgs",
                LongName = "RegEx Single",
                Description = "miau miau miau",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ProjectId = "testProject",
                FieldCategory = "1",
                PrimitiveType = "Chars Array (Str)",
                PrimitiveQuantity = 40,
                ValueRegEx = "^\\[(\"Dante\",?|\"Vergil\",?|\"Nero\",?|\"V\",?)\\]$",
                Default = null,
                Created_at = "2020-07-01 12:07:20",
                Updated_at = null,
                Deleted_at = null,
            };
            formFieldsList.Add(field);
            field = new Web_Field()
            {
                Id = "5",
                Name = "st5",
                LongName = "Test String3",
                Description = "miau miau miau",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = false,
                ProjectId = "testProject",
                FieldCategory = "1",
                PrimitiveType = "Chars Array (Str)",
                PrimitiveQuantity = 5,
                ValueRegEx = null,
                Default = null,
                Created_at = "2020-07-01 12:07:20",
                Updated_at = null,
                Deleted_at = null,
            };
            formFieldsList.Add(field);
            field = new Web_Field()
            {
                Id = "6",
                Name = "st3",
                LongName = "Test String4",
                Description = "miau miau miau",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ProjectId = "testProject",
                FieldCategory = "1",
                PrimitiveType = "Chars Array (Str)",
                PrimitiveQuantity = 3,
                ValueRegEx = null,
                Default = null,
                Created_at = "2020-07-01 12:07:20",
                Updated_at = null,
                Deleted_at = null,
            };
            formFieldsList.Add(field);
        }

        /// <summary>
        /// Change the displayed tab to the one titled "Find Tag"
        /// </summary>
        public static void ShowFind()
        {
            var main = App.Current.MainPage as Rfid.Views.ReadersMainTabbedPage;
            var target = main.Children.Where(x => x.Title == "Find Tag").FirstOrDefault() as NavigationPage;
            main.CurrentPage = target;
            target.CurrentPage.Focus();
        }

        /// <summary>
        /// Change the displayed tab to the one titled "Read Write"
        /// </summary>
        public static void ShowReadWrite()
        {
           var main = App.Current.MainPage as Rfid.Views.ReadersMainTabbedPage;
            var target = main.Children.Where(x => x.Title == "Read Write").FirstOrDefault() as NavigationPage;
            main.CurrentPage = target;
            target.CurrentPage.Focus();
        }


        /// <summary>
        /// Gets the <see cref="ViewModels.ViewModelLocator"/> that will return a ViewModel for a View
        /// </summary>
        public static ViewModelLocator ViewModel { get; } = new ViewModelLocator();

        protected async override void OnStart()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            if (App.IsConnected)
            {
                CosmosDBManager.Init();               
            }
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            if (App.IsConnected)
            {
                try
                {
                    int count = Database._database.Table<PendingOperation>().Count();
                    for (int i = 0; i < count; i++)
                    {
                        PendingOperation opt = Database.GetFirstOperationInQueue();
                        if (opt != null)
                        {
                            KeyValuePair<string, string> isInserted = CosmosDBManager.InsertOneObject(JsonConvert.DeserializeObject<Dictionary<string,object>>(opt.Data));
                            if (isInserted.Key == "Success")
                            {
                                Database.DeletePendingOperation(opt);
                            }
                        }
                    }
                    if (count != 0) MessagingCenter.Send(Current, "PendingUpdated");
                }
                catch (Exception e) 
                {
                    CrossToastPopUp.Current.ShowToastMessage("There was an error uploading operations. Please, restart de app.");
                }
            }
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            App.IsConnected = e.NetworkAccess == NetworkAccess.Internet;
            if (App.IsConnected) CrossToastPopUp.Current.ShowToastMessage("Internet connection established!...Pending operations will be uploaded when app goes background.");
            else CrossToastPopUp.Current.ShowToastMessage("Internet connection lost... Working offline mode from now on.");
        }

    }
}