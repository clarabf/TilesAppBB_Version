﻿using System;
using System.Collections.Generic;
using TilesApp.Services;
using TilesApp.Models.Skeletons;
using Xamarin.Forms;
using TilesApp.Models;

namespace TilesApp.Views
{
    public partial class Reg : BasePage
    {

        private string appName;
        public RegMetaData MetaData { get; set; }
        public Reg(string tag)
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            try
            {
                MetaData = new RegMetaData(OdooXMLRPC.GetAppConfig(tag));
                string[] appNameArr = tag.Split('_');
                MetaData.AppType = appNameArr[1];
                MetaData.AppName = appNameArr[2];
                MetaData.Station = App.Station;
                lblTest.Text = appNameArr[2].ToUpper() + " (REG)";
            }
            catch
            {
                DisplayAlert("Error", "Config file is not valid. Maybe there are syntax issues or one or several field names are duplicated.", "Ok");
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopModalAsync(true);
                });
            }
        }

        public override void ScannerReadDetected(Dictionary<string, object> input)
        {

            foreach (Dictionary<string, object> item in MetaData.ScannerReads)
            {
                if (item[nameof(BaseMetaData.InputDataProps.Value)].ToString() == input[nameof(BaseMetaData.InputDataProps.Value)].ToString())
                {
                    return;
                }
            }

            Dictionary<string, object> returnedData = MetaData.ProcessScannerRead(input);
            if (returnedData.Count > 0)
            {
                if (MetaData.IsValid())
                {
                    lblTitle.IsVisible = true;
                    lblTitleLine.IsVisible = true;
                    lblEmptyView.IsVisible = false;
                    lblEmptyViewAnimation.IsVisible = false;
                    btnSaveAndFinish.IsVisible = true;
                }
                ViewableReads.Add(input[nameof(BaseMetaData.InputDataProps.Value)].ToString());
            }
        }
        private void Delete_ScannerRead(object sender, EventArgs args)
        {
            Button button = (Button)sender;
            string removedObject = button.ClassId;
            // Remove from both the viewable list and the ScannerReads 
            ViewableReads.Remove(button.ClassId);
            if (ViewableReads.Count == 0)
            {
                lblTitle.IsVisible = false;
                lblTitleLine.IsVisible = false;
                lblEmptyView.IsVisible = true;
                lblEmptyViewAnimation.IsVisible = true;
                btnSaveAndFinish.IsVisible = false;
            }
            foreach (Dictionary<string, object> item in MetaData.ScannerReads)
            {
                if (item[nameof(BaseMetaData.InputDataProps.Value)].ToString() == removedObject)
                {
                    MetaData.ScannerReads.Remove(item);
                    return;
                }
            }
        }
        private async void SaveAndFinish(object sender, EventArgs args)
        {
            if (MetaData.IsValid())
            {
                if (CosmosDBManager.InsertOneObject(MetaData))
                {
                    string message = "";
                    foreach (Dictionary<string, object> item in MetaData.ScannerReads)
                    {
                        message += item[nameof(BaseMetaData.InputDataProps.Value)].ToString() + " - ";
                    }
                    await DisplayAlert("Component/s were registered successfully!", message.Substring(0, message.Length - 2), "OK");
                }
                else
                    await DisplayAlert("Component/s were NOT registered successfully!", "We could not connect to the Database Server", "OK");
            }
            else
            {
                await DisplayAlert("Error processing Meta Data!", "Please contact your Odoo administrator", "OK");
            }
            await Navigation.PopModalAsync(true);
        }
        private async void Come_Back(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PopModalAsync(true);
            });
            return true;
        }
    }
}