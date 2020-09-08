using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syncfusion.XForms.ComboBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesApp.Models;
using TilesApp.Models.DataModels;
using TilesApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TilesApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FamilyAndGroups : ContentPage
    {

        private List<Web_Field> formFieldsList = new List<Web_Field>();
        public ObservableCollection<Web_ProtoFamily> FamGroupList { get; set; } = new ObservableCollection<Web_ProtoFamily>();

        List<string> projectNames = new List<string>();
        private bool rememberProject;

        public FamilyAndGroups()
        {
            InitializeComponent();
            
            foreach (Web_Project p in App.Projects) projectNames.Add(p.Name);
            projectDropdown.DataSource = projectNames;
            projectDropdown.SelectionChanged += projectChosen_CommandAsync;

            if (App.CurrentProjectName == null)
            {
                SelectProjectFrame.IsVisible = true;
            }

            MessagingCenter.Subscribe<Application>(Application.Current, "PendingUpdated", (s) =>
            {
                btBadge.BadgeText = "0";
                bdSettings.TextColor = Color.Transparent;
                bdSettings.BackgroundColor = Color.Transparent;
            });

            BindingContext = this;
            App.Inventory.Clear();
        }

        
        async void OnSearchPressed(object sender, EventArgs e)
        {
            FamGroupList.Clear();
            LoadingPopUp.IsVisible = true;
            loading.IsRunning = true;
            bool success = await setFamiliesList();
            LoadingPopUp.IsVisible = false;
            loading.IsRunning = false;
            if (!success) 
            {
                await DisplayAlert("Warning", "No matches found...\n", "Ok");
                Web_ProtoFamily pf = new Web_ProtoFamily()
                {
                    Id = "123",
                    Type = "family",
                    Name = "Fake-object",
                    Description = "Fake object for tests",
                    Slug = "fake_object",
                };
                FamGroupList.Add(pf);
            }

        }
        async void OnCollectionViewSelectionChanged(object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count != 0)
            {
                LoadingPopUp.IsVisible = true;
                loading.IsRunning = true;
                string currentName = (e.CurrentSelection.FirstOrDefault() as Web_ProtoFamily)?.Name;
                string currentId = (e.CurrentSelection.FirstOrDefault() as Web_ProtoFamily)?.Id;
                string currentSlug = (e.CurrentSelection.FirstOrDefault() as Web_ProtoFamily)?.Slug;

                formFieldsList.Clear();
                List<Web_Field> allFields = await Api.GetFieldsList(currentSlug);
                formFieldsList = allFields.FindAll(delegate (Web_Field wf) { return wf.Category != 0; });

                LoadingPopUp.IsVisible = false;
                loading.IsRunning = false;
                cView.SelectedItem = null;
                if (formFieldsList.Count > 0 || currentName == "Fake-object")
                {
                    if (currentName == "Fake-object") fillTestFields();
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new FormPage(currentName, currentId, currentSlug, formFieldsList, new Dictionary<string, object>(), null));
                }
                else await DisplayAlert("Warning", "<" + currentName + "> has no fields...", "Ok");
            }
        }
        private async void Inventory_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (var InputWithDevice in args.NewItems.Cast<Dictionary<string, object>>())
                {
                    searchBar.Text = (string)InputWithDevice["Value"];
                }
            }
            FamGroupList.Clear();
            LoadingPopUp.IsVisible = true;
            loading.IsRunning = true;
            bool success = await setFamiliesList();
            LoadingPopUp.IsVisible = false;
            loading.IsRunning = false;
            if (!success) await DisplayAlert("Warning", "No matches found...", "Ok");
        }
        private async Task<bool> setFamiliesList()
        {
            bool success = false;
            string result = await Api.GetFamiliesList();
            if (result != "")
            {
                List<Dictionary<string, object>> projectsList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(result);

                foreach (Dictionary<string, object> dictP in projectsList)
                {
                    if (dictP["name"].ToString().ToLower().Replace(" ", "").Contains(searchBar.Text.ToLower().Replace(" ", "")))
                    {
                        //Only store the data we need
                        Web_ProtoFamily pf = new Web_ProtoFamily()
                        {
                            Id = dictP["id"].ToString(),
                            Type = dictP["type"].ToString(),
                            Name = dictP["name"].ToString(),
                            Description = dictP["description"].ToString(),
                            Slug = dictP["slug"].ToString(),
                        };
                        FamGroupList.Add(pf);
                        success = true;
                    }
                }
            }
            return success;
        }
        private void fillTestFields()
        {
            Web_Field field = new Web_Field()
            {
                Id = "12345",
                Name = "rgm",
                LongName = "RegEx Multi",
                Description = "miau miau miau",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ProjectId = "testProject",
                Category = 1,
                PrimitiveType = 7,
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
                Id = "67891",
                Name = "rgs",
                LongName = "RegEx Single",
                Description = "miau miau miau",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ProjectId = "testProject",
                Category = 1,
                PrimitiveType = 7,
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
                Id = "78910",
                Name = "st5",
                LongName = "Test String3",
                Description = "miau miau miau",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = false,
                ProjectId = "testProject",
                Category = 1,
                PrimitiveType = 7,
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
                Id = "89101",
                Name = "st3",
                LongName = "Test String4",
                Description = "miau miau miau",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ProjectId = "testProject",
                Category = 1,
                PrimitiveType = 7,
                PrimitiveQuantity = 3,
                ValueRegEx = null,
                Default = null,
                Created_at = "2020-07-01 12:07:20",
                Updated_at = null,
                Deleted_at = null,
            };
            formFieldsList.Add(field);
        }

        #region choose project frame
        private async void projectChosen_CommandAsync(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            SfComboBox comboBox = (SfComboBox)sender;
            Web_Project projectSelected = App.Projects.Find(delegate (Web_Project p) { return p.Name == comboBox.SelectedItem.ToString(); });
            App.CurrentProjectName = projectSelected.Name;
            App.CurrentProjectSlug = projectSelected.Slug;
            btContinue.IsVisible = true;
            btLine.IsVisible = true;
            await DisplayAlert("Current project selected!", "You have selected the project <" + App.CurrentProjectName + "> to work on.", "Ok");
        }

        async void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            rememberProject = e.Value;
        }

        private async void Continue_Command(object sender, EventArgs args)
        {
            if (rememberProject)
            {
                if (Application.Current.Properties.ContainsKey("current_project_name")) Application.Current.Properties["current_project_name"] = App.CurrentProjectName;
                else Application.Current.Properties.Add("current_project_name", App.CurrentProjectName);
                if (Application.Current.Properties.ContainsKey("current_project_slug")) Application.Current.Properties["current_project_slug"] = App.CurrentProjectSlug;
                else Application.Current.Properties.Add("current_project_slug", App.CurrentProjectSlug);
                await Application.Current.SavePropertiesAsync();
            }
            SelectProjectFrame.IsVisible = false;
        }
        #endregion

        #region lower menu commands
        private async void Config_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Configuration());
        }
        private async void Reader_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Rfid.Views.ReadersMainTabbedPage());
        }
        private async void Logout_Command(object sender, EventArgs args)
        {
            if (await DisplayAlert("You are abandoning this page", "Are you sure you want to logout?", "OK", "Cancel"))
            {
                if (App.IsConnected)
                {
                    //CosmosDBManager.InsertOneObject(new AppBasicOperation(AppBasicOperation.OperationType.Logout)); // Register the logout! 
                }
                App.User.UserTokenExpiresAt = DateTime.Now;
                int res = App.Database.SaveUser(App.User);
                Device.BeginInvokeOnMainThread(() =>
                {
                    App.User = new User();
                    App.ActiveSession = false;
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new Main());
                });
            }
        }
        #endregion

        #region overrides
        protected override void OnAppearing()
        {
            App.Inventory.CollectionChanged += Inventory_CollectionChanged;
            //Button badge
            int opt = App.Database.GetOfflineOperationsCount();
            btBadge.BadgeText = opt.ToString();
            if (opt != 0)
            {
                bdSettings.TextColor = Color.White;
                bdSettings.BackgroundColor = Color.Black;
            }
            else
            {
                bdSettings.TextColor = Color.Transparent;
                bdSettings.BackgroundColor = Color.Transparent;
            }
            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            App.Inventory.CollectionChanged -= Inventory_CollectionChanged;
            MessagingCenter.Unsubscribe<Application>(this, "PendingUpdated");
            base.OnDisappearing();
        }
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("You are abandoning this page", "Are you sure you want to logout?", "OK", "Cancel"))
                {
                    if (App.IsConnected)
                    {
                        //CosmosDBManager.InsertOneObject(new AppBasicOperation(AppBasicOperation.OperationType.Logout)); // Register the logout! 
                    }
                    App.User.UserTokenExpiresAt = DateTime.Now;
                    int res = App.Database.SaveUser(App.User);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        App.User = new User();
                        App.ActiveSession = false;
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new Main());
                    });
                }
            });
            return true;
        }
        #endregion
    }
}