using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Syncfusion.XForms.ComboBox;
using TilesApp.Services;
using System.Collections.Generic;
using TilesApp.Models.DataModels;
using Newtonsoft.Json;
using System;

namespace TilesApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Configuration : ContentPage
    {
        List<string> projectNames = new List<string>();
        public Configuration()
        {
            InitializeComponent();
            foreach (Web_Project p in App.Projects) projectNames.Add(p.Name);

            projectDropdown.DataSource = projectNames;
            projectDropdown.SelectionChanged += projectChosen_CommandAsync;
            
            if (App.CurrentProjectName != null)
            {
                lblProject.Text = App.CurrentProjectName;
            }
        } 

        private async void projectChosen_CommandAsync(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            SfComboBox comboBox = (SfComboBox)sender;
            lblProject.Text = comboBox.SelectedItem.ToString();
            Web_Project projectSelected = App.Projects.Find(delegate (Web_Project p) { return p.Name == lblProject.Text; });
            App.CurrentProjectName = projectSelected.Name;
            App.CurrentProjectSlug = projectSelected.Slug;
            await DisplayAlert("Current project changed!", "You have selected the project <" + App.CurrentProjectName + "> to work on", "Ok");
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