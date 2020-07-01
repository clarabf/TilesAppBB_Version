using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesApp.Models;
using TilesApp.Models.DataModels;
using TilesApp.Services;
using TilesApp.Views.Other_Functionalities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TilesApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FamilyAndGroups : ContentPage
    {

        private List<Dictionary<string, object>> protoFamiliesList;
        private List<Dictionary<string, object>> familyFieldsList;
        private List<Dictionary<string, object>> fieldsList;
        private List<Web_Field> formFieldsList = new List<Web_Field>();
        public ObservableCollection<Web_ProtoFamily> FamGroupList { get; set; } = new ObservableCollection<Web_ProtoFamily>();

        public FamilyAndGroups()
        {
            InitializeComponent();
            BindingContext = this;
            App.Inventory.Clear();
        }

        #region lower menu commands
        private async void Config_Command(object sender, EventArgs args)
        {
            //await Navigation.PushModalAsync(new Configuration(this));
            await DisplayAlert("Warning", "Page still in progres...", "Ok");
        }
        private async void Pending_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new PendingOperations());
        }
        private async void Reader_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Rfid.Views.MainPage());
        }
        private async void Logout_Command(object sender, EventArgs args)
        {
            if (await DisplayAlert("You are abandoning this page", "Are you sure you want to logout?", "OK", "Cancel"))
            {
                if (App.IsConnected)
                {
                    CosmosDBManager.InsertOneObject(new AppBasicOperation(AppBasicOperation.OperationType.Logout)); // Register the logout! 
                }
                //timer.Stop();
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
        
        async void OnSearchPressed(object sender, EventArgs e)
        {
            LoadingPopUp.IsVisible = true;
            bool success = await setProtofamiliesList();
            LoadingPopUp.IsVisible = false;
            if (!success) await DisplayAlert("Warning", "No matches found...", "Ok");

        }

        void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string currentName = (e.CurrentSelection.FirstOrDefault() as Web_ProtoFamily)?.Name;
            string currentId = (e.CurrentSelection.FirstOrDefault() as Web_ProtoFamily)?.CosmoId;
            setFormFields(currentId);
            //FOR TESTS
            //fillTestFields();
            Navigation.PopModalAsync(true);
            Navigation.PushModalAsync(new FormPage(currentName, formFieldsList));
        }

        protected override void OnAppearing()
        {
            App.Inventory.CollectionChanged += Inventory_CollectionChanged;
            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            App.Inventory.CollectionChanged -= Inventory_CollectionChanged;
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
                        CosmosDBManager.InsertOneObject(new AppBasicOperation(AppBasicOperation.OperationType.Logout)); // Register the logout! 
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
        private async void Inventory_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (var InputWithDevice in args.NewItems.Cast<Dictionary<string, object>>())
                {
                    searchBar.Text = (string)InputWithDevice["Value"];
                }
            }
            bool success = await setProtofamiliesList();
            if (!success) await DisplayAlert("Warning", "No matches found...", "Ok");
        }
        private async Task<bool> setProtofamiliesList()
        {
            bool success = false;
            string result = await PHPApi.GetProductTypesList();
            if (result != "")
            { 
                FamGroupList.Clear();

                Dictionary<string, object> content = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

                JArray protofamilies = (JArray)content["protofamilies"];
                protoFamiliesList = protofamilies.ToObject<List<Dictionary<string, object>>>();

                JArray familyFields = (JArray)content["family_fields"];
                familyFieldsList = familyFields.ToObject<List<Dictionary<string, object>>>();

                JArray fields = (JArray)content["fields"];
                fieldsList = fields.ToObject<List<Dictionary<string, object>>>();

                foreach (Dictionary<string, object> dictPF in protoFamiliesList)
                {
                    if (dictPF["name"].ToString().ToLower().Replace(" ", "").Contains(searchBar.Text.ToLower().Replace(" ", "")))
                    {
                        Web_ProtoFamily pf = new Web_ProtoFamily()
                        {
                            CosmoId = dictPF["id"].ToString(),
                            ProjectId = dictPF["project_id"].ToString(),
                            CategoryId = dictPF["category_id"].ToString(),
                            Name = dictPF["name"].ToString(),
                            Description = dictPF["description"].ToString(),
                            Slug = dictPF["slug"].ToString(),
                        };
                        if (dictPF["type"] != null) pf.Type = int.Parse(dictPF["type"].ToString());
                        if (dictPF["version_id"] != null) pf.VersionId = int.Parse(dictPF["version_id"].ToString());
                        if (dictPF["version_name"] != null) pf.VersionName = dictPF["version_name"].ToString();
                        if (dictPF["created_at"] != null) pf.Created_at = dictPF["created_at"].ToString();
                        if (dictPF["updated_at"] != null) pf.Updated_at = dictPF["updated_at"].ToString();
                        if (dictPF["deleted_at"] != null) pf.Deleted_at = dictPF["deleted_at"].ToString();
                        FamGroupList.Add(pf);
                        success = true;
                    }
                }
            }
            return success;
        }
        private void setFormFields(string fam_id)
        {
            formFieldsList.Clear();

            List<string> fields = new List<string>();
            foreach (Dictionary<string, object> dictFF in familyFieldsList)
            {
                if (dictFF["protofamily_id"].ToString()==fam_id)
                {
                    fields.Add(dictFF["field_id"].ToString());
                }
            }

            foreach (string fi_id in fields)
            {
                Dictionary<string, object> fieldData = fieldsList.Find(delegate (Dictionary<string, object> dict) { return dict["id"].ToString() == fi_id; });
                //Ignore internal data
                if (Convert.ToInt32(fieldData["field_category"]) != 0)
                {
                    Web_Field field = new Web_Field()
                    {
                        CosmoId = fieldData["id"].ToString(),
                        Name = fieldData["name"].ToString(),
                        LongName = fieldData["long_name"].ToString(),
                        Description = fieldData["description"].ToString(),
                        MongoSlug = fieldData["mongoslug"].ToString(),
                        Slug = fieldData["slug"].ToString(),
                        ValueIsUnique = (bool)fieldData["value_is_unique"],
                        ValueIsRequired = (bool)fieldData["value_is_required"],
                        ValueIsForeignKey = (bool)fieldData["value_is_foreign_key"],
                    };
                    if (fieldData["project_id"] != null) field.ProjectId = fieldData["project_id"].ToString();
                    if (fieldData["field_category"] != null) field.FieldCategory = Convert.ToInt32(fieldData["field_category"]);
                    if (fieldData["variant"] != null) field.Variant = Convert.ToInt32(fieldData["variant"]);
                    if (fieldData["primitive_type"] != null) field.PrimitiveType = Convert.ToInt32(fieldData["primitive_type"]);
                    if (fieldData["primitive_quantity"] != null) field.PrimitiveQuantity = Convert.ToInt32(fieldData["primitive_quantity"]);
                    if (fieldData["value_regex"] != null) field.ValueRegEx = fieldData["value_regex"].ToString();
                    if (fieldData["default"] != null) field.Default = fieldData["default"].ToString();
                    if (fieldData["created_at"] != null) field.Created_at = fieldData["created_at"].ToString();
                    if (fieldData["updated_at"] != null) field.Updated_at = fieldData["updated_at"].ToString();
                    if (fieldData["deleted_at"] != null) field.Deleted_at = fieldData["deleted_at"].ToString();
                    formFieldsList.Add(field);
                }
            }
        }
        private void fillTestFields()
        {
            Web_Field field = new Web_Field()
            {
                CosmoId = "12345",
                Name = "testString",
                LongName = "Test String",
                Description = "miau miau miau",
                MongoSlug = "ts",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ValueIsForeignKey = false,
                ProjectId = "testProject",
                FieldCategory = 1,
                Variant = 2,
                PrimitiveType = 2,
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
                CosmoId = "67891",
                Name = "testString2",
                LongName = "Test String2",
                Description = "miau miau miau",
                MongoSlug = "ts",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ValueIsForeignKey = false,
                ProjectId = "testProject",
                FieldCategory = 1,
                Variant = 2,
                PrimitiveType = 2,
                PrimitiveQuantity = 40,
                ValueRegEx = null,
                Default = null,
                Created_at = "2020-07-01 12:07:20",
                Updated_at = null,
                Deleted_at = null,
            };
            formFieldsList.Add(field);
            field = new Web_Field()
            {
                CosmoId = "78910",
                Name = "testString3",
                LongName = "Test String3",
                Description = "miau miau miau",
                MongoSlug = "ts",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ValueIsForeignKey = false,
                ProjectId = "testProject",
                FieldCategory = 1,
                Variant = 2,
                PrimitiveType = 2,
                PrimitiveQuantity = 40,
                ValueRegEx = null,
                Default = null,
                Created_at = "2020-07-01 12:07:20",
                Updated_at = null,
                Deleted_at = null,
            };
            formFieldsList.Add(field);
            field = new Web_Field()
            {
                CosmoId = "89101",
                Name = "testString4",
                LongName = "Test String4",
                Description = "miau miau miau",
                MongoSlug = "ts",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ValueIsForeignKey = false,
                ProjectId = "testProject",
                FieldCategory = 1,
                Variant = 2,
                PrimitiveType = 2,
                PrimitiveQuantity = 50,
                ValueRegEx = null,
                Default = null,
                Created_at = "2020-07-01 12:07:20",
                Updated_at = null,
                Deleted_at = null,
            };
            formFieldsList.Add(field);
        }
    }
}