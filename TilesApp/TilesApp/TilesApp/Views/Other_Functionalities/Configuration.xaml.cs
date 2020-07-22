using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Syncfusion.XForms.ComboBox;
using TilesApp.Services;
using System.Collections.Generic;
using TilesApp.Models.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace TilesApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Configuration : ContentPage
    {
        public ObservableCollection<string> projectNames { get; set; } = new ObservableCollection<string>();
        public Configuration()
        {
            InitializeComponent();
            
            foreach (Web_Project p in App.Projects) projectNames.Add(p.Name);
            if (App.Projects.Count == 0) projectNames.Add("");

            projectDropdown.DropDownOpen += dropDownOpen_Command;
            projectDropdown.SelectionChanged += projectChosen_Command;

            if (App.CurrentProjectName != null)
            {
                lblProject.Text = App.CurrentProjectName;
            }
            
            BindingContext = this;
        }

        private async void dropDownOpen_Command(object sender, EventArgs e)
        {
            //Case you started the App offline and projets were not recovered in main page.
            if (App.IsConnected && App.Projects.Count == 0)
            {
                LoadingPopUp.IsVisible = true;
                loading.IsRunning = true;
                string result = await Api.GetProjectsList();
                if (result != "")
                {
                    App.Projects = JsonConvert.DeserializeObject<List<Web_Project>>(result);
                    projectNames.Clear();
                    foreach (Web_Project p in App.Projects) projectNames.Add(p.Name);
                }
                LoadingPopUp.IsVisible = false;
                loading.IsRunning = false;
            }
        }

        private async void projectChosen_Command(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            SfComboBox comboBox = (SfComboBox)sender;
            if (comboBox.SelectedItem.ToString() != "")
            {
                lblProject.Text = comboBox.SelectedItem.ToString();
                Web_Project projectSelected = App.Projects.Find(delegate (Web_Project p) { return p.Name == lblProject.Text; });
                App.CurrentProjectName = projectSelected.Name;
                App.CurrentProjectSlug = projectSelected.Slug;
                await DisplayAlert("Current project changed!", "You have selected the project <" + App.CurrentProjectName + "> to work on", "Ok");
            }
        }

        async void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                if (Application.Current.Properties.ContainsKey("current_project_name")) Application.Current.Properties["current_project_name"] = App.CurrentProjectName;
                else Application.Current.Properties.Add("current_project_name", App.CurrentProjectName);
                if (Application.Current.Properties.ContainsKey("current_project_slug")) Application.Current.Properties["current_project_slug"] = App.CurrentProjectSlug;
                else Application.Current.Properties.Add("current_project_slug", App.CurrentProjectSlug);
                await Application.Current.SavePropertiesAsync();
            }
        }
    }
}