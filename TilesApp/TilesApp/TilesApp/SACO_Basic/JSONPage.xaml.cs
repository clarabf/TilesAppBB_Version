using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class JSONPage : ContentPage
    {
        private double width = 0;
        private double height = 0;

        public JSONPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public JSONPage(string json)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            try
            {
                Dictionary<string, object> appDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                List<object> appElements = ((JArray)appDict["e"]).ToObject<List<object>>();

                mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                Label Title = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 22,
                    TextColor = Color.Black,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Text = appDict["ti"].ToString() + "_" + appDict["t"].ToString()
                };
                mainGrid.Children.Add(Title, 0, 0);

                int row = 1;
                foreach (object element in appElements)
                {
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                    Dictionary<string, object> elementData = ((JObject)element).ToObject<Dictionary<string, object>>();
                    switch (elementData["t"].ToString())
                    {
                        case "Barcode":
                            Console.WriteLine(elementData["topText"] + "..." + elementData["bottomText"]);
                            break;
                        case "ComboBox":
                            Console.WriteLine(elementData["ti"] + "...(type):" + elementData["e"].GetType());
                            List<object> combBoxElements = ((JArray)elementData["e"]).ToObject<List<object>>();
                            foreach (object cbElement in combBoxElements)
                            {
                                Dictionary<string, object> cbElementData = ((JObject)cbElement).ToObject<Dictionary<string, object>>();
                                Console.WriteLine("........" + cbElementData["n"] + "..." + cbElementData["color"]);
                            }
                            break;
                        case "Text":
                            Label label = new Label
                            {
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 15,
                                TextColor = Color.Black,
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                                Text = elementData["n"].ToString()
                            };
                            mainGrid.Children.Add(label, 0, row);
                            break;
                        case "Button":
                            Button button = new Button
                            {
                                Text = elementData["n"].ToString(),
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center
                            };
                            if (elementData["f"].ToString() == "Cancel") button.Clicked += Cancel;
                            else if (elementData["f"].ToString() == "NewPage") button.Clicked += NewPage;
                            mainGrid.Children.Add(button, 0, row);
                            break;
                    }

                    row++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            //mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            //mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            //Label nameLabel = new Label
            //{
            //    FontAttributes = FontAttributes.Bold,
            //    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            //    BackgroundColor = Color.Aqua,
            //    VerticalOptions = LayoutOptions.Center,
            //    HorizontalOptions = LayoutOptions.Center,
            //    Text = "PRUEBA"
            //};

            //Label nameLabel2 = new Label
            //{
            //    FontAttributes = FontAttributes.Bold,
            //    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            //    BackgroundColor = Color.Aqua,
            //    VerticalOptions = LayoutOptions.Center,
            //    HorizontalOptions = LayoutOptions.Center,
            //    Text = "PRUEBA2"
            //};

            //Label nameLabel3 = new Label
            //{
            //    FontAttributes = FontAttributes.Bold,
            //    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            //    BackgroundColor = Color.Aqua,
            //    VerticalOptions = LayoutOptions.Center,
            //    HorizontalOptions = LayoutOptions.Center,
            //    Text = "PRUEBA3"
            //};

            //mainGrid.Children.Add(nameLabel, 0, 0);
            //mainGrid.Children.Add(nameLabel2, 0, 1);
            //mainGrid.Children.Add(nameLabel3, 0, 2);
        }

        private async void Cancel(object sender, EventArgs args)
        {
            await DisplayAlert("Cancel", "Goodbye!", "OK");
        }

        private async void NewPage(object sender, EventArgs args)
        {
            await DisplayAlert("New Page", "Hello new page!", "OK");
        }

    }
}
