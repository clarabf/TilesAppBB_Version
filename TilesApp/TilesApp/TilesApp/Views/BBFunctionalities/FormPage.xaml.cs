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

            foreach (Web_Field field in _formFields)
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
                    case "Integer":
                        if (field.PrimitiveQuantity == 1)
                        {
                            Picker picker = new Picker
                            {
                                ClassId = field.Id,
                                BackgroundColor = Color.Transparent,
                                Title = field.LongName,
                                VerticalOptions = LayoutOptions.StartAndExpand
                            };
                            picker.Items.Add("True");
                            picker.Items.Add("False");
                            elementsGrid.Children.Add(picker, 0, row);
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
            for (int i = row; i < 12; i++)
            {
                elementsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }
        }

        private void selectionChanged_command(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            SfComboBox comboBox = (SfComboBox)sender;
            List<int> indexList = (List<int>)comboBox.SelectedIndices;
            if (indexList.Count == 0)
            {
                //comboBox.Clear();
                //Web_Field field = _formFields.Find(delegate (Web_Field wf) { return wf.Id == comboBox.ClassId; });
                //Dictionary<string, object> result = FormatRegex(field.ValueRegEx);
                //List<string> items = (List<string>)result["options"];
                //items.Add("ahaaaaaaaaaaaaaaaaaaaaaaa");
                //comboBox.ComboBoxSource = items;
            }
        }

        private async void entryCompleted_command(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;
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
                string current_label = "";
                Dictionary<string, object> formInfo = new Dictionary<string, object>();
                formInfo.Add("element",lblTitle.Text);
                for (int i = 0; i < _formFields.Count * 2; i++)
                {
                    string elementType = elementsGrid.Children.ElementAt(i).GetType().ToString();
                    switch (elementType)
                    {
                        case "Xamarin.Forms.Label":
                            Label label = (Label)elementsGrid.Children.ElementAt(i);
                            current_label = label.Text;
                            break;
                        case "Xamarin.Forms.Entry":
                            Entry entry = (Entry)elementsGrid.Children.ElementAt(i);
                            Web_Field field = _formFields.Find(delegate (Web_Field wf) { return wf.Id == entry.ClassId; });

                            Debug.WriteLine(entry.Placeholder + "..." + entry.Text);
                            if (field.ValueIsRequired && (entry.Text == null || entry.Text == "")) errors.Add("<" + current_label + "> cannot be empty.");
                            else formInfo.Add(field.Name, entry.Text);
                            break;
                        case "Xamarin.Forms.Picker":
                            Picker picker = (Picker)elementsGrid.Children.ElementAt(i);
                            field = _formFields.Find(delegate (Web_Field wf) { return wf.Id == picker.ClassId; });
                            
                            Debug.WriteLine(picker.Title + "..." + picker.SelectedItem);
                            if (field.ValueIsRequired && picker.SelectedItem == null) errors.Add("<" + current_label + "> cannot be empty.");
                            else formInfo.Add(field.Name, picker.SelectedItem);
                            break;
                        case "Syncfusion.XForms.ComboBox.SfComboBox":
                            SfComboBox comboBox = (SfComboBox)elementsGrid.Children.ElementAt(i);
                            field = _formFields.Find(delegate (Web_Field wf) { return wf.Id == comboBox.ClassId; });

                            Dictionary<string, object> result = FormatRegex(field.ValueRegEx);

                            if ((bool)result["multi"])
                            {
                                List<int> indexList = (List<int>)comboBox.SelectedIndices;
                                if (field.ValueIsRequired && indexList == null)
                                {
                                    errors.Add("<" + current_label + "> cannot be empty.");
                                }
                                else
                                {
                                    if (indexList.Count > 0)
                                    {
                                        List<string> items = (List<string>)result["options"];
                                        Collection<string> selectedItems = new Collection<string>();

                                        string options = "";

                                        foreach (int ix in indexList)
                                        {
                                            options += items[ix] + ", ";
                                            selectedItems.Add(items[ix]);
                                        }
                                        Debug.WriteLine(options);
                                        formInfo.Add(field.Name, selectedItems);
                                    }
                                    else errors.Add("<" + current_label + "> cannot be empty.");
                                }
                            }
                            else
                            {
                                Debug.WriteLine(comboBox.Watermark + "..." + comboBox.SelectedItem);
                                if (field.ValueIsRequired && comboBox.SelectedItem == null) errors.Add("<" + current_label + "> cannot be empty.");
                                else formInfo.Add(field.Name, comboBox.SelectedItem);
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
                    }
                    await DisplayAlert("SENDING FORM", mess, "Ok");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
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