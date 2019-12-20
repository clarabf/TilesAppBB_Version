using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TilesApp.Rfid;
using TilesApp.Rfid.Models;
using TilesApp.Rfid.ViewModels;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOAssociate : BasePage
    {
        private double width = 0;
        private double height = 0;
        public ObservableCollection<string> InputDataValues { get; set; } = new ObservableCollection<string>();
        public SACOAssociate()
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;
        }

        public override void InputDataDetected(Dictionary<string, object> input)
        {
            lblBarcode.IsVisible = true;
            entry.IsVisible = true;
            btnSaveAndFinish.IsVisible = true;
            barcode.Text = input[nameof(InputDataProps.ReaderType)].ToString() +"|"+ input[nameof(InputDataProps.Value)].ToString();
            InputDataValues.Add(input[nameof(InputDataProps.Value)].ToString());
        }


        private async void SaveAndFinish(object sender, EventArgs args)
        {
            //Update info in DB
            if (entry.Text != "")
            {
                await DisplayAlert("Component added successfully!", "<" + entry.Text + "> stored in DB.", "OK");
                await Navigation.PopModalAsync(true);
            }
            else
            {
                await DisplayAlert("Empty name", "Please, fill the component name before clicking 'Save & Finish'.", "OK");
            }
        }

        private async void Cancel(object sender, EventArgs args)
        {
            MessagingCenter.Unsubscribe<Application, String>(Application.Current, "BarcodeScanned");
            await Navigation.PopModalAsync(true);
        }


    }
}
