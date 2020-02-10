using System;
using System.Collections.Generic;
using TilesApp.Services;
using TilesApp.Models.Skeletons;
using Xamarin.Forms;
using TilesApp.Models;
using System.Collections.ObjectModel;
using System.Dynamic;

namespace TilesApp.Views
{
    public partial class Review : BasePage
    {

        public ReviewMetaData MetaData { get; set; }
        public ObservableCollection<ViewableElement> Elements { get; set; } = new ObservableCollection<ViewableElement>();

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
        public async void FetchData(string barcode) {
            List<Dictionary<string, object>> data = await CosmosDBManager.FetchData(barcode, MetaData.Apps);
            foreach (var dict in data)
            {
                ViewableElement elm = new ViewableElement();
                try
                {
                    foreach (KeyValuePair<string, object> item in dict)
                    {
                        switch (item.Key)
                        {
                            case "AppName":
                                elm.AppName = item.Value.ToString();
                                break;
                            case "AppType":
                                elm.AppType = item.Value.ToString();
                                break;
                            case "ScannerReads":
                                var scannerReads = (List<object>)item.Value;
                                foreach (var sr in scannerReads)
                                {
                                    IDictionary<string, object> scannerRead = (ExpandoObject)sr;
                                    try
                                    {
                                        if (((string)scannerRead["Value"]).Equals(barcode))
                                    {
                                            elm.Time = (DateTime)scannerRead["Timestamp"];
                                        }
                                    }

                                    catch
                                    {
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    Elements.Add(elm);
                }
                catch (Exception e)
                {
                    await DisplayAlert("Error",e.Message,"OK");
                }
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


    public class ViewableElement
    {
        public string AppType { get; set; }
        public string AppName { get; set; }
        public DateTime Time { get; set; }
    }
}