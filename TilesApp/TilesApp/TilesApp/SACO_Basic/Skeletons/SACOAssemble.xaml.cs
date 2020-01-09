﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TilesApp.SACO
{
    public partial class SACOAssemble : BasePage
    {
        private bool mainScanned = false;
        private string mainCode;
        private List<string> barcodes = new List<string>();
        private string appName;
        public ObservableCollection<string> BarcodesScanned { get; set; } = new ObservableCollection<string>();

        public SACOAssemble(string name)
        {
            InitializeComponent();
            BindingContext = this;
            appName = name;
            lblTest.Text = appName + " (Assemble)";
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public override void ScannerReadDetected(Dictionary<string, object> input)
        {
            if (!mainScanned)
            {
                mainScanned = true;
                mainCode = input[nameof(InputDataProps.Value)].ToString();
                lblTitle.Text = "Scan barcode of the other components";
                BarcodesScanned.Add("Main item <" + mainCode + "> scanned (" + DateTime.Now.ToShortTimeString() + ")");
                btnSaveAndFinish.IsVisible = true;
            }
            else
            {
                barcodes.Add(input[nameof(InputDataProps.Value)].ToString());
                lblTitle.Text = "Scan barcode of the other components (" + barcodes.Count + ")";
                BarcodesScanned.Add("Item <" + input[nameof(InputDataProps.Value)].ToString() + "> scanned (" + DateTime.Now.ToShortTimeString() + ")");
            }
        }
        private async void SaveAndFinish(object sender, EventArgs args)
        {
            //Update info in DB
            string message = "";
            foreach (string code in barcodes) message += code + " - ";
            await DisplayAlert(mainCode + " was assembled successfully!", message.Substring(0, message.Length - 2), "OK");
            await Navigation.PopModalAsync(true);
        }

        private async void Cancel(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

    }
}