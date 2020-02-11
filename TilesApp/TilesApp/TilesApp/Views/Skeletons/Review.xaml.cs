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

        public Review(string tag)
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            try
            {
                MetaData = new ReviewMetaData(OdooXMLRPC.GetAppConfig(tag));
                string[] appNameArr = tag.Split('_');
                MetaData.AppType = appNameArr[1];
                MetaData.AppName = appNameArr[2];
                if (MetaData.Station == null) MetaData.Station = App.Station;
                lblTest.Text = appNameArr[2].ToUpper() + " (REVIEW)";
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
                lblTitle.IsVisible = true;
                lblTitleLine.IsVisible = true;
                lblEmptyView.IsVisible = false;
                lblEmptyViewAnimation.IsVisible = false;
                ViewableReads.Add(input[nameof(BaseMetaData.InputDataProps.Value)].ToString());
                FetchData(input[nameof(BaseMetaData.InputDataProps.Value)].ToString());

            }
        }
        public async void FetchData(string barcode)
        {
            List<Dictionary<string, object>> data = await CosmosDBManager.FetchData(barcode, MetaData.Apps);
            foreach (var dict in data)
            {
                try
                {
                    foreach (var pair in dict)
                    {
                        if (pair.Key.Equals("ScannerReads"))
                        {
                            foreach (var item in (List<object>)pair.Value)
                            {
                                IDictionary<string, object> scannerRead = (IDictionary<string, object>)item;
                                if (barcode.Equals((string)scannerRead["Value"]))
                                    dict.Add("OperationAt", (DateTime)scannerRead["Timestamp"]);
                            }
                        }
                    }
                }
                catch
                {
                }
                Elements.Add(dict);
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