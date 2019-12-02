using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOReader : ContentPage
    {
        private double width = 0;
        private double height = 0;
        private bool mainScanned = false;
        private string mainCode;
        private List<string> barcodes = new List<string>();
        public ObservableCollection<string> BarcodesScanned { get; set; } = new ObservableCollection<string>();

        public SACOReader()
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;
            BarcodesScanned.Add("Main item <" + mainCode + "> scanned (" + DateTime.Now.ToShortTimeString() + ")");
            MessagingCenter.Subscribe<Application, String>(Application.Current, "SendBarcode", (s, a) => {
                if (!mainScanned)
                {
                    mainScanned = true;
                    mainCode = a.ToString();
                    title.Text = "Scan QR of the component";
                    BarcodesScanned.Add("Main item <" + mainCode + "> scanned (" + DateTime.Now.ToShortTimeString() + ")");
                    //label.Text = "Main item <" + mainCode + "> identified successfully.\n" + DateTime.Now + "\nStart associating components.";
                    btnSaveAndFinish.IsVisible = true;
                }
                else
                {
                    barcodes.Add(a.ToString());
                    title.Text = "Scan QR of the component (" + barcodes.Count + ")";
                    //label.Text = "Item <" + a.ToString() + "> identified successfully.\n" + DateTime.Now + "\nKeep scanning components.";
                    BarcodesScanned.Add("Item <" + a.ToString() + "> scanned (" + DateTime.Now.ToShortTimeString() + ")");
                }
            });
        }

        private async void SaveAndFinish(object sender, EventArgs args)
        {
            MessagingCenter.Unsubscribe<Application, String>(Application.Current, "SendBarcode");
            //Update info in DB
            string message="";
            foreach (string code in barcodes) message += code + " - ";
            await DisplayAlert(mainCode  + " was assembled successfully!", message.Substring(0, message.Length-2), "OK");
            await Navigation.PopModalAsync(true);
        }

        private async void Cancel(object sender, EventArgs args)
        {
            MessagingCenter.Unsubscribe<Application, String>(Application.Current, "SendBarcode");
            await Navigation.PopModalAsync(true);
        }

    }
}
