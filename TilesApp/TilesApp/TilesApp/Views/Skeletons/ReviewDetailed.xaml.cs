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
        protected int counter = 0;
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
                if(kvp.Key.ToString() == "AppName")
                {
                    lblTitle.Text = kvp.Value.ToString().ToUpper() + " REVIEW";
                }
                else
                {
                    //adding row dynamically
                    int rowHeight = 40;
                    rowHeight = kvp.Key.ToString() == "Address" ? 100 : rowHeight;
                    Details.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight) });

                    //key label
                    Label lblKey = new Label();                
                    lblKey.FontSize = 14;
                    lblKey.FontAttributes = FontAttributes.Bold;
                    lblKey.TextColor = Color.White;
                    lblKey.Padding = new Thickness(15, 8);
                    lblKey.Text = kvp.Key.ToString().ToUpper();
                    Details.Children.Add(lblKey, 0, counter);
                
                    //value label
                    Label lblValue = new Label();
                    lblValue.FontSize = 14;
                    lblValue.TextColor = Color.White;
                    lblValue.Padding = new Thickness(8);
                    lblValue.FontFamily = "sans-serif-light";
                    lblValue.Text = kvp.Value.ToString();
                    Details.Children.Add(lblValue, 1, counter);
                    counter++;
                }               
                
            }
            
        }

    }
}