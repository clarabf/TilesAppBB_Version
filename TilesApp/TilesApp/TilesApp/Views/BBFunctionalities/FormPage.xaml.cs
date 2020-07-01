using System;
using Xamarin.Forms;
using TilesApp.Models.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace TilesApp.Views
{
    public partial class FormPage : ContentPage
    {
        public FormPage(string title, List<Web_Field> formFields)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            int row = 0;
            lblTitle.Text = title.ToUpper();
            foreach (Web_Field field in formFields)
            {
                elementsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                Label lblElement = new Label
                {
                    Text = field.LongName,
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    FontSize = 18,
                };
                elementsGrid.Children.Add(lblElement, 0, row);
                row++;
                switch (field.PrimitiveType)
                {
                    //integer
                    case 0:
                        if (field.PrimitiveQuantity == 1)
                        {
                            Picker picker = new Picker
                            {
                                ClassId = field.CosmoId,
                                BackgroundColor = Color.White,
                                Title = field.LongName,
                                VerticalOptions = LayoutOptions.StartAndExpand
                            };
                            picker.Items.Add("True");
                            picker.Items.Add("False");
                            elementsGrid.Children.Add(picker, 0, row);
                            row++;
                        }
                        break;
                    case 1:
                        break;
                    //string
                    case 2:
                        if (field.ValueRegEx != null)
                        {
                            Dictionary<string, object> result = FormatRegex(field.ValueRegEx);
                            if ((bool)result["multi"])
                            {
                                List<string> items = (List<string>)result["options"];
                                //SfComboBox multiComboBox = new SfComboBox
                                //{
                                //    ClassId = field.CosmoId,
                                //    BackgroundColor = Color.White,
                                //    HeightRequest = 40,
                                //    MultiSelectMode = MultiSelectMode.Delimiter,
                                //    Delimiter = ",",
                                //    DataSource = items
                                //};
                                //elementsGrid.Children.Add(multiComboBox, 0, row);
                                //row++;
                            }
                            else
                            {
                                Picker picker = new Picker
                                {
                                    ClassId = field.CosmoId,
                                    BackgroundColor = Color.White,
                                    Title = field.LongName,
                                    VerticalOptions = LayoutOptions.StartAndExpand,
                                };
                                List<string> items = (List<string>)result["options"];
                                foreach (string it in items)
                                {
                                    picker.Items.Add(it);
                                }
                                elementsGrid.Children.Add(picker, 0, row);
                                row++;
                            }
                        }
                        else
                        {
                            Entry entry = new Entry
                            {
                                ClassId = field.CosmoId,
                                BackgroundColor = Color.White,
                                Placeholder = field.LongName + " (max. " + field.PrimitiveQuantity + ")",
                                VerticalOptions = LayoutOptions.StartAndExpand
                            };
                            elementsGrid.Children.Add(entry, 0, row);
                            row++;
                        }
                        break;
                }
            }
            for (int i = row; i < 12; i++)
            {
                elementsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopModalAsync(true);
            Navigation.PushModalAsync(new FamilyAndGroups());
            return true;
        }

        private Dictionary<string, object> FormatRegex(string regEx)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            string filteredRefEx = "";

            //Single selection or multi selection
            if (regEx.Contains("*\\]$"))
            {
                filteredRefEx = regEx.Replace(",?)*\\]$", "");
                result.Add("multi", true);
            }
            else
            {
                filteredRefEx = regEx.Replace(",?)\\]$", "");
                result.Add("multi", false);
            }

            filteredRefEx = filteredRefEx.Substring(4).Replace(",?", "").Replace("\"", "");
            string[] options = filteredRefEx.Split('|');
            result.Add("options", options.ToList());

            return result;
        }
    }
}