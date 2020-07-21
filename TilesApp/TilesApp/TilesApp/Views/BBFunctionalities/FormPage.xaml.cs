using System;
using Xamarin.Forms;
using TilesApp.Models.DataModels;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.XForms.ComboBox;
using System.Diagnostics;
using TilesApp.Services;
using System.Collections.ObjectModel;

namespace TilesApp.Views
{
    public partial class FormPage : ContentPage
    {

        List<Web_Field> _formFields;

        public FormPage(string title, List<Web_Field> formFields)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            
            _formFields = formFields;
            lblTitle.Text = title.ToUpper();
            int row = 0;
            string asterix;

            foreach (Web_Field field in _formFields)
            {
                asterix = "";
                if (field.ValueIsRequired) asterix = " *";
                
                elementsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                Label lblElement = new Label
                {
                    Text = field.LongName + asterix,
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
                    case "Integer":
                        if (field.PrimitiveQuantity == 1)
                        {
                            TokenSettings tokenSettings = new TokenSettings
                            {
                                BackgroundColor = Color.Black,
                                TextColor = Color.White,
                                IsCloseButtonVisible = false,
                            };
                            SfComboBox comboBox = new SfComboBox
                            {
                                ClassId = field.Id,
                                TextColor = Color.Black,
                                BackgroundColor = Color.Transparent,
                                ShowClearButton = true,
                                IsEditableMode = false,
                                EnableAutoSize = true,
                                IsSelectedItemsVisibleInDropDown = false,
                                TokensWrapMode = TokensWrapMode.None,
                                TokenSettings = tokenSettings,
                                Watermark = "Select one...",
                                MultiSelectMode = MultiSelectMode.None
                            };
                            comboBox.ComboBoxSource = new List<string>(){"true", "false"};
                            comboBox.SelectionChanged += selectionChanged_command;
                            elementsGrid.Children.Add(comboBox, 0, row);
                            row++;
                        }
                        break;
                    //string
                    case "Chars Array (Str)":
                        if (field.ValueRegEx != null)
                        {
                            Dictionary<string, object> result = FormatRegex(field.ValueRegEx);
                            
                            List<string> items = (List<string>)result["options"];

                            TokenSettings tokenSettings = new TokenSettings
                            {
                                BackgroundColor = Color.Black,
                                TextColor = Color.White,
                                IsCloseButtonVisible = false,
                            };

                            SfComboBox comboBox = new SfComboBox
                            {
                                ClassId = field.Id,
                                TextColor = Color.Black,
                                BackgroundColor = Color.Transparent,
                                ShowClearButton = true,
                                IsEditableMode = false,
                                EnableAutoSize = true,
                                IsSelectedItemsVisibleInDropDown = false,
                                TokensWrapMode = TokensWrapMode.None,
                                ComboBoxSource = items,
                                TokenSettings = tokenSettings,
                            };

                            if ((bool)result["multi"])
                            {
                                comboBox.Watermark = "Select one at least...";
                                comboBox.MultiSelectMode = MultiSelectMode.Token;
                            }
                            else
                            {
                                comboBox.Watermark = "Select one...";
                                comboBox.MultiSelectMode = MultiSelectMode.None;
                            }

                            comboBox.SelectionChanged += selectionChanged_command;
                            var i = comboBox.Text;
                            elementsGrid.Children.Add(comboBox, 0, row);
                            row++;
                        }
                        else
                        {
                            Entry entry = new Entry
                            {
                                ClassId = field.Id,
                                BackgroundColor = Color.Transparent,
                                PlaceholderColor = Color.Gray,
                                Placeholder = field.LongName + " (max. " + field.PrimitiveQuantity + ")",
                                VerticalOptions = LayoutOptions.StartAndExpand
                            };
                            entry.Completed += entryCompleted_command;
                            elementsGrid.Children.Add(entry, 0, row);
                            row++;
                        }
                        break;
                }
            }
            for (int i = row; i < 9; i++)
            {
                elementsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }
        }

        private void selectionChanged_command(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            SfComboBox comboBox = (SfComboBox)sender;
            comboBox.BorderColor = Color.Black;
        }

        private async void entryCompleted_command(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;
            entry.PlaceholderColor = Color.Gray;
            Web_Field field = _formFields.Find(delegate (Web_Field wf) { return wf.Id == entry.ClassId; });
            if (entry.Text.Length > field.PrimitiveQuantity) 
            {
                await DisplayAlert("Warning", 
                    "Element <" + field.Name + "> exceeds in " + (entry.Text.Length - field.PrimitiveQuantity) + " characters (max. " + field.PrimitiveQuantity + ")",
                    "Ok");
                entry.Text = null;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopModalAsync(true);
            Navigation.PushModalAsync(new FamilyAndGroups());
            return true;
        }

        private async void Send_Command(object sender, EventArgs args)
        {
            try
            {
                List<string> errors = new List<string>();
                Dictionary<string, List<object>> formInfo = new Dictionary<string, List<object>>();
                formInfo.Add("element", new List<object> { lblTitle.Text });
                for (int i = 0; i < _formFields.Count * 2; i++)
                {
                    string elementType = elementsGrid.Children.ElementAt(i).GetType().ToString();
                    switch (elementType)
                    {
                        case "Xamarin.Forms.Entry":
                            Entry entry = (Entry)elementsGrid.Children.ElementAt(i);
                            Web_Field field = _formFields.Find(delegate (Web_Field wf) { return wf.Id == entry.ClassId; });

                            Debug.WriteLine(entry.Placeholder + "..." + entry.Text);
                            // If entry remains null, we do not store the info.
                            if (entry.Text == null || entry.Text == "")
                            {
                                if (field.ValueIsRequired)
                                {
                                    entry.PlaceholderColor = Color.Red;
                                    errors.Add("<" + field.LongName + "> cannot be empty.");
                                }
                            }
                            else
                            {
                                if (entry.Text.Length > field.PrimitiveQuantity)
                                {
                                    entry.PlaceholderColor = Color.Red;
                                    errors.Add("<" + field.LongName + "> exceeds in " + (entry.Text.Length - field.PrimitiveQuantity) + " characters (max. " + field.PrimitiveQuantity + ")");
                                    entry.Text = null;
                                }
                                else formInfo.Add(field.Name, new List<object> { entry.Text });
                            }
                            break;
                        case "Syncfusion.XForms.ComboBox.SfComboBox":
                            SfComboBox comboBox = (SfComboBox)elementsGrid.Children.ElementAt(i);
                            field = _formFields.Find(delegate (Web_Field wf) { return wf.Id == comboBox.ClassId; });

                            Dictionary<string, object> result = FormatRegex(field.ValueRegEx);

                            if ((bool)result["multi"])
                            {
                                List<int> indexList = (List<int>)comboBox.SelectedIndices;
                                if (indexList == null)
                                {
                                    if (field.ValueIsRequired)
                                    {
                                        comboBox.BorderColor = Color.Red;
                                        errors.Add("<" + field.LongName + "> cannot be empty.");
                                    }
                                }
                                else
                                {
                                    if (indexList.Count > 0)
                                    {
                                        List<string> items = (List<string>)result["options"];
                                        List<object> selectedItems = new List<object>();

                                        string options = "";

                                        foreach (int ix in indexList)
                                        {
                                            options += items[ix] + ", ";
                                            selectedItems.Add(items[ix]);
                                        }
                                        Debug.WriteLine(options);
                                        formInfo.Add(field.Name, selectedItems);
                                    }
                                    else
                                    {
                                        comboBox.BorderColor = Color.Red;
                                        errors.Add("<" + field.LongName + "> cannot be empty.");
                                    }

                                }
                            }
                            else
                            {
                                Debug.WriteLine(comboBox.Watermark + "..." + comboBox.SelectedItem);
                                if (comboBox.SelectedItem == null) 
                                {
                                    if (field.ValueIsRequired)
                                    {
                                        comboBox.BorderColor = Color.Red;
                                        errors.Add("<" + field.LongName + "> cannot be empty.");
                                    }
                                }
                                else formInfo.Add(field.Name, new List<object> { comboBox.SelectedItem });
                            }
                            break;
                    }
                }
                string message = "";
                if (errors.Count > 0)
                {
                    foreach (string eMessage in errors) message += eMessage + "\n";
                    await DisplayAlert("Some fields missing...", message, "Ok");
                }
                else
                {
                    KeyValuePair<string, string> result = CosmosDBManager.InsertOneObject(new Form_Info(formInfo));
                    string mess  = "";
                    if (result.Key == "Success")
                    {
                        if (result.Value == "Online") mess = "Your form has been correctly sent!";
                        else mess = "You are offline. The form has been stored and will be sent when you are connected.";
                        CleanForm();
                    }
                    else mess = "There was an error sending the form. Please, contact with IT...";
                    await DisplayAlert("SENDING FORM", mess, "Ok");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void CleanForm()
        {
            for (int i = 0; i < _formFields.Count * 2; i++)
            {
                string elementType = elementsGrid.Children.ElementAt(i).GetType().ToString();
                switch (elementType)
                {
                    case "Xamarin.Forms.Entry":
                        Entry entry = (Entry)elementsGrid.Children.ElementAt(i);
                        entry.Text = null;
                        break;
                    case "Xamarin.Forms.Picker":
                        Picker picker = (Picker)elementsGrid.Children.ElementAt(i);
                        picker.SelectedItem = null;
                        break;
                    case "Syncfusion.XForms.ComboBox.SfComboBox":
                        SfComboBox comboBox = (SfComboBox)elementsGrid.Children.ElementAt(i);
                        comboBox.Text = null;
                        comboBox.Clear();
                        break;
                }
            }
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