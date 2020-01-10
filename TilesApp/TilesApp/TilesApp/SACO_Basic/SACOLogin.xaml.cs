using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TilesApp.Azure;
using TilesApp.Rfid;
using Xamarin.Forms;
using XmlRpc;

namespace TilesApp.SACO
{

    public partial class SACOLogin : BasePage
    {
        private Dictionary<string, object> users;
        public SACOLogin()
        {
            InitializeComponent();
            Setup();
            NavigationPage.SetHasNavigationBar(this, false);

            Dictionary<string, object> metaData = new Dictionary<string, object>();
            metaData.Add("asdsdssds", "vvvvvv");
            metaData.Add("hhhhhh", "v2v2v2");
            metaData.Add("time_stamp", DateTime.Now);
            bool success = CosmosDBManager.InsertOneObject(metaData);

            OdooConnection od = new OdooConnection();
            //od.CreateLog();
            users = od.GetUsers();
            if (users.ContainsKey("error"))
            {
                string message="";
                if (users["error"].ToString() == "internet") message = "Please, check that internet is turned on in your mobile and restart the application.";
                else if (users["error"].ToString() == "odoo") message = "Odoo connection failed. Please, restart the application.";
                DisplayAlert("Error recovering users", message, "OK");
            }
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
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        Navigation.PushModalAsync(new SACOAppPage(userInfo));
                    });
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
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PopModalAsync(true);
                Navigation.PushModalAsync(new SACOScan("SCAN YOUR EMPLOYEE CARD", 1, users));
            });
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
