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
    public partial class ReviewDetailed : ContentPage
    {
        protected Dictionary<string, object> _dict;
        protected string _barcode;
        public Dictionary<string, object> Dict
        {
            get => this._dict;
            set => this._dict =  value;
        }
        public string Barcode
        {
            get => this._barcode;
            set => this._barcode = value;
        }

        public ReviewDetailed(Dictionary<string, object> dict,string barcode)
        {
            Dict = dict;
            Barcode = barcode;
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        protected override void OnAppearing()
        {
            IDictionary<string, object> loc = new Dictionary<string, object>();
            if (Dict != null && Dict.Count > 0)
            {
                foreach (KeyValuePair<string, object> kvp in Dict)
                {
                    switch (kvp.Key)
                    {
                        case "Location":
                            try
                            {
                                loc = (IDictionary<string,object>)kvp.Value;
                                KeyValuePair<string, object> tempKvp = new KeyValuePair<string, object>("Address", (string)loc["display_name"]);
                                ProcessValue(tempKvp);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            break;
                        case "_id":
                        case "_t":
                        case "ScannerReads":
                        case "UserId":
                        case "DeviceSerialNumber":
                        case "CustomFields":
                            break;
                        default:
                            ProcessValue(kvp);
                            break;
                    }
                }

            }
        }
        private void ProcessValue(KeyValuePair<string, object> kvp)
        {

            if (kvp.Value is Dictionary<string, object>)
            {
                foreach (KeyValuePair<string, object> item in (Dictionary<string, object>)kvp.Value)
                {
                    ProcessValue(item);
                }
            }
            else
            {
                Label lbl = new Label();
                lbl.FontSize = 14;
                lbl.TextColor = Color.White;
                lbl.Padding = 3;
                lbl.Text = kvp.Key + ": " + kvp.Value.ToString();
                Details.Children.Add(lbl);
                BoxView bx = new BoxView();
                bx.HeightRequest = 0.5;
                bx.Color = Color.White;
                bx.Opacity = 40;
                Details.Children.Add(bx);
            }
            
        }



    }
}