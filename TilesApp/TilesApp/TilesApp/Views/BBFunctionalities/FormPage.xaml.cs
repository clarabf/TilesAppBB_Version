using System;
using Xamarin.Forms;
using TilesApp.Models.DataModels;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.XForms.ComboBox;
using System.Diagnostics;
using TilesApp.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Java.Lang;

namespace TilesApp.Views
{
    public partial class FormPage : ContentPage
    {

        List<Web_Field> _formFields;
        string _jsonFields;
        Dictionary<string, object> _oldFormInfo = new Dictionary<string, object>();
        Dictionary<string, object> formInfo = new Dictionary<string, object>();
        string __fn;
        string _tp;
        string __fs;
        PendingOperation _pendingOperation;

        public FormPage(string familyName, string familyType, string familySlug, 
            string jsonFields,
            List<Web_Field> formFields, 
            Dictionary<string,object> oldFormInfo,
            PendingOperation pendingOperation)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            _tp = familyType;
            __fn = familyName;
            __fs = familySlug;
            _jsonFields = jsonFields;
            _formFields = formFields;
            _oldFormInfo = oldFormInfo;
            _pendingOperation = pendingOperation;

            lblTitle.Text = familyName.ToUpper();
            int row = 0;
            string asterix;

            foreach (Web_Field field in _formFields)
            {
                if (field.Category != 0)
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
                    //CAREFUL: Error if the user connects offline...
                    PrimitiveType p = App.PrimitiveTypes[field.PrimitiveType.ToString()];
                    switch (p.Csharp_name)
                    {
                        //integer
                        case "Boolean":
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
                                    ClassId = field.Slug,
                                    TextColor = Color.Black,
                                    TextSize = 14,
                                    VerticalOptions = LayoutOptions.StartAndExpand,
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
                                comboBox.ComboBoxSource = new List<string>() { "true", "false" };
                                comboBox.SelectionChanged += selectionChanged_command;

                                // Fill the field if we have a previous value
                                if (_oldFormInfo.ContainsKey(field.Slug)) comboBox.SelectedItem = _oldFormInfo[field.Slug];

                                elementsGrid.Children.Add(comboBox, 0, row);
                                row++;
                            }
                            break;
                        //string
                        case "String":
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
                                    ClassId = field.Slug,
                                    TextColor = Color.Black,
                                    TextSize = 14,
                                    VerticalOptions = LayoutOptions.StartAndExpand,
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

                                // Fill the field if we have a previous value
                                if (_oldFormInfo.ContainsKey(field.Slug)) comboBox.SelectedItem = _oldFormInfo[field.Slug];

                                elementsGrid.Children.Add(comboBox, 0, row);
                                row++;
                            }
                            else
                            {
                                CustomEntry entry = new CustomEntry
                                {
                                    ClassId = field.Slug,
                                    FontSize = 14,
                                    BackgroundColor = Color.Transparent,
                                    PlaceholderColor = Color.Gray,
                                    Placeholder = field.LongName + " (max. " + field.PrimitiveQuantity + ")",
                                    VerticalOptions = LayoutOptions.StartAndExpand
                                };
                                entry.Completed += entryCompleted_command;

                                // Fill the field if we have a previous value
                                if (_oldFormInfo.ContainsKey(field.Slug)) entry.Text = _oldFormInfo[field.Slug].ToString();

                                elementsGrid.Children.Add(entry, 0, row);
                                row++;
                            }
                            break;
                        default:
                            CustomEntry defEntry = new CustomEntry
                            {
                                ClassId = field.Slug,
                                FontSize = 14,
                                BackgroundColor = Color.Transparent,
                                PlaceholderColor = Color.Gray,
                                Placeholder = field.LongName + " (max. " + field.PrimitiveQuantity + ")",
                                VerticalOptions = LayoutOptions.StartAndExpand
                            };
                            defEntry.Completed += entryCompleted_command;

                            // Fill the field if we have a previous value
                            if (_oldFormInfo.ContainsKey(field.Slug)) defEntry.Text = _oldFormInfo[field.Slug].ToString();

                            elementsGrid.Children.Add(defEntry, 0, row);
                            row++;
                            break;
                    }
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
            Web_Field field = _formFields.Find(delegate (Web_Field wf) { return wf.Slug == entry.ClassId; });
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

            LoadingPopUp.IsVisible = true;
            loading.IsRunning = true;
            CheckFields();
            LoadingPopUp.IsVisible = false;
            loading.IsRunning = false;
        }

        private async void CheckFields() 
        {
            try
            {
                List<string> errors = new List<string>();
                formInfo.Clear();
                LoadingPopUp.IsVisible = true;
                loading.IsRunning = true;
                for (int i = 0; i < _formFields.Count * 2; i++)
                {
                    string elementType = elementsGrid.Children.ElementAt(i).GetType().ToString();
                    switch (elementType)
                    {
                        case "TilesApp.CustomEntry":
                            CustomEntry entry = (CustomEntry)elementsGrid.Children.ElementAt(i);
                            Web_Field field = _formFields.Find(delegate (Web_Field wf) { return wf.Slug == entry.ClassId; });

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
                                else 
                                {
                                    PrimitiveType p = App.PrimitiveTypes[field.PrimitiveType.ToString()];
                                    int num;
                                    if (p.Csharp_name == "Integer")
                                    {
                                        if (int.TryParse(entry.Text, out num)) formInfo.Add(field.Name, num);
                                        else errors.Add("<" + field.LongName + "> should be a numerical value.");
                                    }
                                    else formInfo.Add(field.Name, entry.Text);
                                }
                            }
                            break;
                        case "Syncfusion.XForms.ComboBox.SfComboBox":
                            SfComboBox comboBox = (SfComboBox)elementsGrid.Children.ElementAt(i);
                            field = _formFields.Find(delegate (Web_Field wf) { return wf.Slug == comboBox.ClassId; });

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
                                        formInfo.Add(field.Name, string.Join(",", selectedItems.ToArray()));
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
                                else
                                {
                                    if (comboBox.SelectedItem != "") formInfo.Add(field.Name, comboBox.SelectedItem);
                                }
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
                    AddPrivateFields();
                    KeyValuePair<string, string> result = CosmosDBManager.InsertAndUpdateOneObject(formInfo, _jsonFields);
                    string messTitle = "";
                    string messContent = "";
                    if (result.Key == "Success")
                    {
                        messTitle = "SUCCESS";
                        if (result.Value == "Online") messContent = "Your form has been correctly sent!";
                        else messContent = "You are offline. The form has been stored and will be sent when you are connected.";
                        CleanForm();
                        if (_pendingOperation != null) App.Database.DeletePendingOperation(_pendingOperation);
                    }
                    else
                    {
                        messTitle = "ERROR";
                        messContent = "There was an error sending the form. Please, contact with IT...";
                    }
                    await DisplayAlert(messTitle, messContent, "Ok");
                }
            }
            catch 
            {
                await DisplayAlert("ERROR", "There was an error sending the form. Please, contact with IT...", "Ok");
            }
        }

        private void AddPrivateFields()
        {
            formInfo.Add(Keys.User, App.User.MSID);
            formInfo.Add(Keys.Type, _tp);
            formInfo.Add(Keys.Phase, (long)1);
            formInfo.Add(Keys.FormName, __fn);
            formInfo.Add(Keys.FormSlug, __fs);
            if (_oldFormInfo.Count == 0) formInfo.Add(Keys.Version, (long)1);
            else formInfo.Add(Keys.Version, (long)int.Parse(_oldFormInfo[Keys.Version].ToString())+1);
        }

        private void CleanForm()
        {
            for (int i = 0; i < _formFields.Count * 2; i++)
            {
                string elementType = elementsGrid.Children.ElementAt(i).GetType().ToString();
                switch (elementType)
                {
                    case "TilesApp.CustomEntry":
                        CustomEntry entry = (CustomEntry)elementsGrid.Children.ElementAt(i);
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
            _oldFormInfo.Clear();
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