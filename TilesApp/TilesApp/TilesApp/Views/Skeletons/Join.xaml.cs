using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TilesApp.Services;
using TilesApp.Models.Skeletons;
using Xamarin.Forms;
using TilesApp.Models;

namespace TilesApp.Views
{
    public partial class Join : BasePage
    {
        public JoinMetaData MetaData { get; set; }
        public Join(string tag)
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            try
            {
                MetaData = new JoinMetaData(OdooXMLRPC.GetAppConfig(tag));
                string[] appNameArr = tag.Split('_');
                MetaData.AppType = appNameArr[1];
                MetaData.AppName = appNameArr[2];
                if (MetaData.Station == null) MetaData.Station = App.Station;
                lblTest.Text = appNameArr[2].ToUpper() + " (JOIN)";
            }
            catch
            {
                DisplayAlert("Error", "Config file is not valid. Maybe there are syntax issues or one or several field names are duplicated.", "Ok");
                Device.BeginInvokeOnMainThread(async () =>
                {
                    CleanReaders();
                    await Navigation.PopModalAsync(true);
                });
            }
        }

        public override void ScannerReadDetected(Dictionary<string, object> input)
        {
            if (MetaData.ParentUUID != null)
            {
                if (MetaData.ParentUUID == input[nameof(BaseMetaData.InputDataProps.Value)].ToString()) return;
            }
            foreach (string item in ViewableReads)
            {
                if (item.Equals(input[nameof(BaseMetaData.InputDataProps.Value)].ToString()))
                {
                    return;
                }
            }
            Dictionary<string, object> processedInput = MetaData.ProcessScannerRead(input);
            if (processedInput.Count > 1)
            {
                bool isParent = false;
                try
                {
                    isParent = (bool)processedInput["IsParent"];
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                if (isParent)
                {

                    lblParentBarcode.Text = ParentDelBtn.ClassId =  input[nameof(BaseMetaData.InputDataProps.Value)].ToString();
                    btnSaveAndFinish.IsVisible = true;
                    lblParent.IsVisible = true;
                    lblTitle.IsVisible = true;
                    lblTitleLine.IsVisible = true;
                    lblEmptyView.IsVisible = false;
                    lblEmptyViewAnimation.IsVisible = false;
                }
                else
                {
                    ViewableReads.Add(input[nameof(BaseMetaData.InputDataProps.Value)].ToString());

                    lblTitle.IsVisible = true;
                    lblTitleLine.IsVisible = true;
                    lblEmptyView.IsVisible = false;
                    lblEmptyViewAnimation.IsVisible = false;
                }
            }
            else
            {
                try
                {
                    DisplayAlert("Error", processedInput["Error"].ToString(), "Ok");
                }
                catch
                {
                }
            }
        }

        private void Delete_ScannerRead(object sender, EventArgs args)
        {
            App.ViewModel.Inventory.ClearCommand.Execute(null);
            Button button = (Button)sender;
            string removedObject = button.ClassId;
            // Remove from both the viewable list and the ScannerReads 
            if (ViewableReads.Contains(button.ClassId))
            {
                ViewableReads.Remove(button.ClassId);
            } else if (lblParentBarcode.Text.Equals(button.ClassId)) {
                lblParent.IsVisible = false;
                lblParentBarcode.Text = "";
                MetaData.ParentUUID = null;
                btnSaveAndFinish.IsVisible = false;
            }
            if (ViewableReads.Count == 0)
            {
                btnSaveAndFinish.IsVisible = false;
                if (lblParentBarcode.Text == "")
                {
                    lblEmptyView.IsVisible = true;
                    lblEmptyViewAnimation.IsVisible = true;
                    lblTitle.IsVisible = false;
                    lblTitleLine.IsVisible = false;
                }               
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
            List<string> errorsList = MetaData.IsValid();
            if (errorsList.Count==0)
            {
                if (CosmosDBManager.InsertOneObject(MetaData))
                {
                    string message = "";
                    foreach (Dictionary<string, object> item in MetaData.ScannerReads)
                    {
                        message += item[nameof(BaseMetaData.InputDataProps.Value)].ToString() + " - ";
                    }
                    await DisplayAlert(MetaData.ParentUUID + " was assembled successfully!", message.Substring(0, message.Length - 2), "OK");
                    btnSaveAndFinish.IsVisible = false;
                    lblEmptyView.IsVisible = true;
                    lblEmptyViewAnimation.IsVisible = true;
                    lblTitle.IsVisible = false;
                    lblTitleLine.IsVisible = false;
                    lblParent.IsVisible = false;
                    ViewableReads.Clear();
                    MetaData.ScannerReads.Clear();
                }
                else
                    await DisplayAlert(MetaData.ParentUUID + " was NOT assembled successfully!", "We could not connect to the Database Server", "OK");
            }
            else
            {
                //CleanReaders();
                string message = "The following fields are not completed:\n";
                foreach (string error in errorsList) message += error + ", ";
                await DisplayAlert("Error processing Meta Data!", message.Substring(0, message.Length-1), "OK");
                //await Navigation.PopModalAsync(true);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                CleanReaders();
                await Navigation.PopModalAsync(true);
            });
            return true;
        }

    }
}