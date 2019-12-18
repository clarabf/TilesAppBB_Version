using Android.Hardware.Usb;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TechnologySolutions.Rfid;
using TilesApp.Rfid;
using TilesApp.Rfid.ViewModels;
using Xamarin.Forms;
using XmlRpc;

namespace TilesApp.SACO
{

    public partial class SACOLogin : ContentPage
    {
        private double width = 0;
        private double height = 0;
        private Dictionary<string, object> users;
        public SACOLogin()
        {
            InitializeComponent();
            Setup();
            NavigationPage.SetHasNavigationBar(this, false);
            OdooConnection od = new OdooConnection();
            users = od.GetUsers();
            width = this.Width;
            height = this.Height;            
            MessagingCenter.Subscribe<Application, UsbDevice>(Application.Current, "DeviceAttached", async (s, device) => {
                if (device != null) {
                    App.ViewModel.Readers.SerialReaders.Add(device);
                }
                   /* await DisplayAlert("Device plugged in", 
                    "Class : "+ device.Class +"\n"+
                    "DeviceClass : " + device.DeviceClass + "\n"+
                    "DeviceId : " + device.DeviceId + "\n" +
                    "DeviceName : " + device.DeviceName + "\n" +
                    "DeviceProtocol : " + device.DeviceProtocol + "\n" +
                    "DeviceSubClass : " + device.DeviceSubclass + "\n" +
                    "Type : " + device.GetType() + "\n" +
                    "ManufacturerName : " + device.ManufacturerName + "\n" +
                    "ProductId : " + device.ProductId + "\n" +
                    "ProductName : " + device.ProductName + "\n" +
                    "SerialNumber : " + device.SerialNumber + "\n" +
                    "VendorId : " + device.VendorId + "\n" +
                    "Version : " + device.Version + "\n" 
                    , "Close alert");           */       
            });

            MessagingCenter.Subscribe<Application, UsbDevice>(Application.Current, "DeviceDetached", async (s, device) => {
                if (device != null) {
                
                }
            });

            

            MessagingCenter.Subscribe<Application, String>(Application.Current, "UserScanned", async (s, a) => {
                await DisplayAlert("User <" + a.ToString() + "> scanned", "Please, wait until your App Page loads", "OK");

                try
                {
                    Dictionary<string, object> userInfo = (Dictionary<string, object>)users[a.ToString()];
                    
                    //OdooConnection oc = new OdooConnection();
                    //Dictionary<string, object> userInfo = oc.GetUserInfo(a.ToString());

                    //Register login
                    //HttpClient client = new HttpClient();
                    //var dataLogin = new Dictionary<string, object>();
                    //dataLogin.Add("id", userInfo["id"].ToString());
                    //dataLogin.Add("cardCode", a.ToString());
                    //dataLogin.Add("employeeName", userInfo["name"].ToString());
                    //dataLogin.Add("timestamp", DateTime.Now);
                    //var content = new StringContent(JsonConvert.SerializeObject(dataLogin), Encoding.UTF8, "application/json");
                    //var postResponse = await client.PostAsync("https://sacoerpconnect.azurewebsites.net/api/insertLoginRecord/", content);
                    //var answer = await postResponse.Content.ReadAsStringAsync();
                    await Navigation.PushModalAsync(new SACOAppPage(userInfo));
                }
                catch
                {
                    await DisplayAlert("Error scanning badge", "User not found in DB...", "Ok");
                }
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Setup();
        }

        private async void GoToScan(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new SACOScan("SCAN YOUR EMPLOYEE CARD",1, users));
        }

        private async void Reader_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Rfid.Views.MainPage());
        }

        private void Setup()
        {
            this.BindWithLifecycle(App.ViewModel.Inventory);
        }
    }
}
