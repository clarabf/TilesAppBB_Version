using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesApp.Models.DataModels;
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

        async void OnCollectionViewSelectionChanged(object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count != 0)
            {
                string data = (e.CurrentSelection.FirstOrDefault() as PendingOperation)?.Data;
                Dictionary<string, object> fields = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                string message = "";
                foreach (KeyValuePair<string, object> kv in fields)
                {
                    if (Keys.ValidKey(kv.Key)) message += kv.Key + ": " + kv.Value + "\n";
                }
                await DisplayAlert("Form selected", message, "Ok");
                cView.SelectedItem = null;
            }
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