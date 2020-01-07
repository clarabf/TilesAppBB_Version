using Newtonsoft.Json;
using PCLAppConfig;
using System;
using System.Collections.Generic;
using TilesApp.Rfid;
using Xamarin.Forms;
using XmlRpc;

namespace TilesApp.SACO
{

    public partial class SACOLogin : BasePage
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
            if (users==null)
            {
                DisplayAlert("Error recovering users", "Please, check that internet is turned on in your mobile and restart the application.", "OK");
                //DisplayAlert("Error recovering users", ConfigurationManager.AppSettings["key"], "OK");
            }
            width = this.Width;
            height = this.Height;            
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
