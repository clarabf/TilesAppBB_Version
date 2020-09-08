using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesApp.Models.DataModels;
using TilesApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TilesApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PendingOperations : ContentPage
    {
        List<PendingOperation> opts = new List<PendingOperation>();
        public ObservableCollection<PendingOperation> PendingOpts { get; set; } = new ObservableCollection<PendingOperation>();
        public PendingOperations()
        {
            InitializeComponent();
            BindingContext = this;
            opts = App.Database.GetPendingOperations();
        }

        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PendingOperation po = e.CurrentSelection.FirstOrDefault() as PendingOperation;
                if (e.CurrentSelection.Count != 0)
                {
                    string data = po.Data;
                    Dictionary<string, object> fields = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    string message = "";
                    foreach (KeyValuePair<string, object> kv in fields)
                    {
                        if (Keys.ValidKey(kv.Key)) message += kv.Key + ": " + kv.Value + "\n";
                    }
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (await DisplayAlert("Form selected", message, "Edit", "Cancel"))
                        {
                            if (App.IsConnected)
                            {
                                string currentName = fields[Keys.FormName].ToString();
                                string currentId = fields[Keys.Type].ToString();
                                string currentSlug = fields[Keys.FormSlug].ToString();

                                List<Web_Field> formFieldsList = new List<Web_Field>();
                                List<Web_Field> allFields = await Api.GetFieldsList(currentSlug);
                                formFieldsList = allFields.FindAll(delegate (Web_Field wf) { return wf.Category != 0; });

                                if (formFieldsList.Count > 0)
                                {
                                    Navigation.PopModalAsync(true);
                                    Navigation.PushModalAsync(new FormPage(currentName, currentId, currentSlug, formFieldsList, fields, po));
                                }
                            }
                            else
                            {
                                await DisplayAlert("Error", "You are offline, so form fields cannot be recovered...", "Ok");
                            }
                        }
                    });
                }
            }
            catch (Exception exc)
            {
                await DisplayAlert("Error", exc.Message, "Ok");
            }
            cView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (opts.Count > 0)
            {
                lblTitleLine.IsVisible = true;
            }
            foreach (PendingOperation opt in opts)
            {
                PendingOpts.Add(opt);
            }
        }
    }
}