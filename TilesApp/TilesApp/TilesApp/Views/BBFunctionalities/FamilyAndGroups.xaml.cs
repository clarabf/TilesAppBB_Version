using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        async void OnSearchPressed(object sender, EventArgs e)
        {
            FamGroupList.Clear();
            LoadingPopUp.IsVisible = true;
            bool success = await setProjectsList();
            LoadingPopUp.IsVisible = false;
            if (!success)
            {
                await DisplayAlert("Warning", "No matches found...\nCreating fake object for tests.", "Ok");
                Web_ProtoFamily pf = new Web_ProtoFamily()
                {
                    CosmoId = "123",
                    ProjectId = "1234",
                    CategoryId = "bla",
                    Name = "Fake-object",
                    Description = "Fake object for tests",
                    Slug = "fake_object",
                };
                FamGroupList.Add(pf);
            }

        }
        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string currentName = (e.CurrentSelection.FirstOrDefault() as Web_ProtoFamily)?.Name;
            string currentId = (e.CurrentSelection.FirstOrDefault() as Web_ProtoFamily)?.CosmoId;
            string currentSlug = (e.CurrentSelection.FirstOrDefault() as Web_ProtoFamily)?.Slug;
            string result =  await Api.GetFieldsList(currentSlug);
            setFormFields(result);
            //FOR TESTS
            fillTestFields();
            Navigation.PopModalAsync(true);
            Navigation.PushModalAsync(new FormPage(currentName, formFieldsList));
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
            string result = await Api.GetProductTypesList();
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

        private async Task<bool> setProjectsList()
        {
            bool success = false;
            string result = await Api.GetProjectsList();
            if (result != "")
            {
                List<Dictionary<string, object>> projectsList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(result);

                foreach (Dictionary<string, object> dictP in projectsList)
                {
                    if (dictP["name"].ToString().ToLower().Replace(" ", "").Contains(searchBar.Text.ToLower().Replace(" ", "")))
                    {
                        Web_ProtoFamily pf = new Web_ProtoFamily()
                        {
                            CosmoId = dictP["id"].ToString(),
                            Name = dictP["name"].ToString(),
                            Slug = dictP["slug"].ToString(),
                        };
                        FamGroupList.Add(pf);
                        success = true;
                    }
                }
            }
            return success;
        }
        private void setFormFields(string jsonFields)
        {
            try
            {
                formFieldsList.Clear();

                Dictionary<string, Dictionary<string,object>> keyField = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(jsonFields);
                foreach (KeyValuePair<string, Dictionary<string, object>> kv in keyField)
                {
                    Dictionary<string, object> fieldData = kv.Value;
                    if (Convert.ToInt32(fieldData["field_category"]) != 0)
                    {
                        Web_Field field = new Web_Field()
                        {
                            Id = fieldData["id"].ToString(),
                            Name = fieldData["name"].ToString(),
                            LongName = fieldData["long_name"].ToString(),
                            Description = fieldData["description"].ToString(),
                            Slug = fieldData["slug"].ToString(),
                            ValueIsUnique = (bool)fieldData["value_is_unique"],
                            ValueIsRequired = (bool)fieldData["value_is_required"],
                            Index = (bool)fieldData["index"],
                            Show = (bool)fieldData["show"],
                            Edit = (bool)fieldData["edit"],
                            Delete = (bool)fieldData["delete"],
                        };
                        //string
                        if (fieldData["project_id"] != null) field.ProjectId = fieldData["project_id"].ToString();
                        if (fieldData["entity_id"] != null) field.EntityId = fieldData["entity_id"].ToString();
                        if (fieldData["protofamily_id"] != null) field.ProtoFamilyId = fieldData["protofamily_id"].ToString();
                        if (fieldData["field_id"] != null) field.FieldId = fieldData["field_id"].ToString();
                        if (fieldData["phase"] != null) field.Phase = fieldData["phase"].ToString();
                        if (fieldData["value_regex"] != null) field.ValueRegEx = fieldData["value_regex"].ToString();
                        if (fieldData["created_at"] != null) field.Created_at = fieldData["created_at"].ToString();
                        if (fieldData["updated_at"] != null) field.Updated_at = fieldData["updated_at"].ToString();
                        if (fieldData["deleted_at"] != null) field.Deleted_at = fieldData["deleted_at"].ToString();
                        //int
                        if (fieldData["field_category"] != null) field.FieldCategory = Convert.ToInt32(fieldData["field_category"]);
                        if (fieldData["primitive_type"] != null) field.PrimitiveType = Convert.ToInt32(fieldData["primitive_type"]);
                        if (fieldData["primitive_quantity"] != null) field.PrimitiveQuantity = Convert.ToInt32(fieldData["primitive_quantity"]);
                        if (fieldData["ui_index"] != null) field.UIindex = Convert.ToInt32(fieldData["ui_index"]);
                        if (fieldData["entity_type"] != null) field.EntityType = Convert.ToInt32(fieldData["entity_type"]);
                        if (fieldData["default"] != null) field.Default = fieldData["default"].ToString();
                        formFieldsList.Add(field);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private void fillTestFields()
        {
            Web_Field field = new Web_Field()
            {
                Id = "12345",
                Name = "regM",
                LongName = "RegEx Multi",
                Description = "miau miau miau",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ProjectId = "testProject",
                FieldCategory = 1,
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
                Name = "regS",
                LongName = "RegEx Single",
                Description = "miau miau miau",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ProjectId = "testProject",
                FieldCategory = 1,
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
                Name = "testString3",
                LongName = "Test String3",
                Description = "miau miau miau",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = false,
                ProjectId = "testProject",
                FieldCategory = 1,
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
                Name = "testString4",
                LongName = "Test String4",
                Description = "miau miau miau",
                Slug = "test-string",
                ValueIsUnique = true,
                ValueIsRequired = true,
                ProjectId = "testProject",
                FieldCategory = 1,
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

        #region overrides
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
        #endregion
    }
}