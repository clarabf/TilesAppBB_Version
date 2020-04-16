using System;
using System.Collections.Generic;
using TilesApp.Services;
using TilesApp.Models.Skeletons;
using Xamarin.Forms;
using TilesApp.Models;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Text;

namespace TilesApp.Views
{
    public partial class FamilyList : ContentPage
    {
        protected ObservableCollection<Dictionary<string, object>> _items;
        public ObservableCollection<Dictionary<string, object>> Items
        {
            get => this._items;
            set {
                this._items = value;             
            }           
        }
        public string ConfigFileName;
        public string RootPageBarcode { get; set; }
        public FamilyList(ObservableCollection<Dictionary<string, object>> items, Dictionary<string,object> parentPageIsRoot,string configFileName)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Items = items;
            ConfigFileName = configFileName;
            try
            {
                RootPageBarcode = (string)parentPageIsRoot["Value"];
            }
            catch {
                RootPageBarcode = "";
            }
        }
        protected override void OnAppearing()
        {            
            Cview.SelectedItem = null;
            Cview.ItemsSource = Items;
            Cview.SelectionChanged += OnItemSelected;
        }
        protected override void OnDisappearing()
        {
            Cview.SelectionChanged -= OnItemSelected;
        }

        void OnItemSelected(object sender, SelectionChangedEventArgs args)
        {
            try
            {
                var selectedItem = args.CurrentSelection[0] as Dictionary<string, object>;
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (RootPageBarcode.Equals("")|| RootPageBarcode.Equals((string)selectedItem["Value"])) {
                        Navigation.PopModalAsync(true);
                    }
                    Navigation.PopModalAsync(true);
                    Navigation.PushModalAsync(new Review(ConfigFileName, (string)selectedItem["Value"]));
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }



    }
}