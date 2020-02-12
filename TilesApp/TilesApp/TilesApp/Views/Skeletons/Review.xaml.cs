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
    public partial class Review : BasePage
    {

        public ReviewMetaData MetaData { get; set; }
        public ObservableCollection<Dictionary<string, object>> Elements { get; set; } = new ObservableCollection<Dictionary<string, object>>();
        public string Barcode { get; set; }
        public Dictionary<string, object> ParentDict { get; set; } = new Dictionary<string, object>();
        public ObservableCollection<Dictionary<string, object>> ChildrenDicts { get; set; } = new ObservableCollection<Dictionary<string, object>>();
        public List<string> ChildrenCodes { get; set; } = new List<string>();
        public Review(string tag)
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            try
            {
                MetaData = new ReviewMetaData(OdooXMLRPC.GetAppConfig(tag));
                string[] appNameArr = tag.Split('_');
                MetaData.AppType = appNameArr[1].ToUpper();
                MetaData.AppName = appNameArr[2].ToUpper();
                if (MetaData.Station == null) MetaData.Station = App.Station;
                lblApp.Text = appNameArr[2].ToUpper() + " (REVIEW)";
            }
            catch
            {
                DisplayAlert("Error", "Config file is not valid. Maybe there are syntax issues or one or several field names are duplicated.", "Ok");
                Device.BeginInvokeOnMainThread(async () =>
                {
                    CleanReaders();
                    await Navigation.PopModalAsync(true);
                });
            }
        }
        public Review(string tag, string barcode) : this(tag)
        {
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("Value", barcode);
            ScannerReadDetected(input);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Cview.SelectedItem = null;
            Cview.SelectionChanged += OnItemSelected;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Cview.SelectionChanged -= OnItemSelected;
        }
        public override void ScannerReadDetected(Dictionary<string, object> input)
        {

            foreach (Dictionary<string, object> item in MetaData.ScannerReads)
            {
                if (item[nameof(BaseMetaData.InputDataProps.Value)].ToString() == input[nameof(BaseMetaData.InputDataProps.Value)].ToString())
                {
                    return;
                }
            }

            Dictionary<string, object> returnedData = MetaData.ProcessScannerRead(input);
            if (returnedData.Count > 0)
            {
                string barcode = input[nameof(BaseMetaData.InputDataProps.Value)].ToString();
                lblTitle.Text = "ITEM HISTORY(.." + barcode.Substring(barcode.Length - 4) + ")";
                lblTitle.IsVisible = true;
                lblTitleLine.IsVisible = true;
                lblEmptyView.IsVisible = false;
                lblEmptyViewAnimation.IsVisible = false;
                Barcode = barcode;
                FetchData(barcode);

            }
        }
        public async void FetchData(string barcode)
        {
            List<Dictionary<string, object>> data = await CosmosDBManager.FetchData(barcode, MetaData.Apps);
            foreach (var dict in data.ToArray())
            {
                try
                {
                    KeyValuePair<string,object> operationAt;
                    foreach (var pair in dict)
                    {
                        if (pair.Key.Equals("ScannerReads"))
                        {
                            foreach (var item in (List<object>)pair.Value)
                            {
                                IDictionary<string, object> scannerRead = (IDictionary<string, object>)item;
                                if (barcode.Equals((string)scannerRead["Value"]))
                                    operationAt = new KeyValuePair<string, object>("OperationAt",(DateTime)scannerRead["Timestamp"]);                                
                            }
                        }
                    }
                    if(operationAt.Key !=null)
                    dict.Add(operationAt.Key,operationAt.Value);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Elements.Add(dict);
                // PROCESS PARENT AND CHILDREN
                try
                {
                    if (((string)dict["AppType"]).Equals("Join"))
                    {
                        if (!((string)dict["ParentUUID"]).Equals(barcode))
                        {
                            ParentDict.Add("Value", (string)dict["ParentUUID"]);
                            try
                            {
                                ParentDict.Add("Timestamp", dict["OperationAt"]);
                                btParent.IsVisible = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                        else
                        {
                            foreach (var item in (List<object>)dict["ScannerReads"])
                            {
                                Dictionary<string, object> scannerRead = new Dictionary<string, object>((IDictionary<string, object>)item);
                                if (!barcode.Equals((string)scannerRead["Value"]))
                                {
                                    if (!ChildrenCodes.Contains((string)scannerRead["Value"]))
                                    {
                                        ChildrenDicts.Add(scannerRead);
                                        ChildrenCodes.Add((string)scannerRead["Value"]);
                                        btChildren.IsVisible = true;
                                    }
                                }
                            }
                        }                        
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        async void OnItemSelected(object sender, SelectionChangedEventArgs args){
            try
            {
                var selectedItem = args.CurrentSelection[0] as Dictionary<string, object>;
                await Navigation.PushModalAsync(new ReviewDetailed(selectedItem, Barcode));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        async void goToFamily(object sender, EventArgs args) {
            Button b = (Button)sender;
            if (b.ClassId == "btParent")
            {
                ObservableCollection<Dictionary<string, object>> tempList = new ObservableCollection<Dictionary<string, object>>();
                tempList.Add(ParentDict);
                await Navigation.PushModalAsync(new FamilyList(tempList));
            }
            else if(b.ClassId == "btChildren")
            {
                await Navigation.PushModalAsync(new FamilyList(ChildrenDicts));
            }
                
        }
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                CleanReaders();
                await Navigation.PopModalAsync(true);
            });
            return true;
        }
    }
}