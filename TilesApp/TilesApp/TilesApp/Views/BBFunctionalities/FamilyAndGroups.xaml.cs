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
            bool success = await setProtofamiliesList();
            if (!success) await DisplayAlert("Warning", "No matches found...", "Ok");

        }

        void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string currentName = (e.CurrentSelection.FirstOrDefault() as Web_ProtoFamily)?.Name;
            string currentId = (e.CurrentSelection.FirstOrDefault() as Web_ProtoFamily)?.CosmoId;
            //DisplayAlert("Hello!", "You've selected <" + currentName + "." + currentId + ">!", "Ok");
            Navigation.PopModalAsync(true);
            Navigation.PushModalAsync(new FormPage(currentName));
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
    }
}