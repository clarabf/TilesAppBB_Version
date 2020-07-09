using System;
using Xamarin.Forms;
using TilesApp.Models.DataModels;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.XForms.ComboBox;
using System.Diagnostics;

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
                    case 0:
                        if (field.PrimitiveQuantity == 1)
                        {
                            Picker picker = new Picker
                            {
                                ClassId = field.CosmoId,
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
                                SfComboBox multiComboBox = new SfComboBox
                                {
                                    ClassId = field.CosmoId,
                                    Text = "Select one...",
                                    TextColor = Color.Gray,
                                    BackgroundColor = Color.Transparent,
                                    HeightRequest = 40,
                                    ShowClearButton = false,
                                    MaximumDropDownHeight = 200,
                                    IsEditableMode = false,
                                    EnableSelectionIndicator = true,
                                    MultiSelectMode = MultiSelectMode.Token,
                                    IsSelectedItemsVisibleInDropDown = false,
                                    ComboBoxSource = items
                                };
                                TokenSettings token = new TokenSettings();
                                token.BackgroundColor = Color.Black;
                                token.TextColor = Color.White;
                                token.IsCloseButtonVisible = true;
                                multiComboBox.TokenSettings = token;
                                elementsGrid.Children.Add(multiComboBox, 0, row);
                                row++;
                            }
                            else
                            {
                                Picker picker = new Picker
                                {
                                    ClassId = field.CosmoId,
                                    BackgroundColor = Color.Transparent,
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

        private void command_removing(object sender, ElementEventArgs e)
        {
            SfComboBox comboBox = (SfComboBox)sender;
            if (comboBox.ComboBoxSource.Count == 0)
            {
                comboBox.Text = "Select one...";
                comboBox.TextColor = Color.Gray;
            }
        }

        private async void entryCompleted_command(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;
            Web_Field field = _formFields.Find(delegate (Web_Field wf) { return wf.CosmoId == entry.ClassId; });
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
                            Web_Field field = _formFields.Find(delegate (Web_Field wf) { return wf.CosmoId == entry.ClassId; });

                            Debug.WriteLine(entry.Placeholder + "..." + entry.Text);
                            if (field.ValueIsRequired && (entry.Text == null || entry.Text == "")) errors.Add("<" + current_label + "> cannot be empty.");
                            break;
                        case "Xamarin.Forms.Picker":
                            Picker picker = (Picker)elementsGrid.Children.ElementAt(i);
                            field = _formFields.Find(delegate (Web_Field wf) { return wf.CosmoId == picker.ClassId; });
                            
                            Debug.WriteLine(picker.Title + "..." + picker.SelectedItem);
                            if (field.ValueIsRequired && picker.SelectedItem == null) errors.Add("<" + current_label + "> cannot be empty.");
                            break;
                        case "Syncfusion.XForms.ComboBox.SfComboBox":
                            SfComboBox multiComboBox = (SfComboBox)elementsGrid.Children.ElementAt(i);
                            field = _formFields.Find(delegate (Web_Field wf) { return wf.CosmoId == multiComboBox.ClassId; });

                            List<int> indexList = (List<int>)multiComboBox.SelectedIndices;
                            if (field.ValueIsRequired && indexList == null)
                            {
                                errors.Add("<" + current_label + "> cannot be empty.");
                            }
                            else
                            {
                                if (indexList.Count > 0)
                                {
                                    Dictionary<string, object> result = FormatRegex(field.ValueRegEx);
                                    List<string> items = (List<string>)result["options"];

                                    string options = "";

                                    foreach (int ix in indexList)
                                    {
                                        options += items[ix] + ", ";
                                    }
                                    Debug.WriteLine(options);
                                }
                                else errors.Add("<" + current_label + "> cannot be empty.");
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
                    await DisplayAlert("SENDING FORM", "'Send form' action still in progres...", "Ok");
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