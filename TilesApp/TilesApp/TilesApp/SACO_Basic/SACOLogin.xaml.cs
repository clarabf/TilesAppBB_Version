using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TilesApp.Models;
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
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public SACOLogin(Dictionary<string, object> usersList)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            users = usersList;
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
                    await Navigation.PushModalAsync(new SACOTests(userInfo));
                }
                catch
                {
                    await DisplayAlert("Error scanning badge", "User not found in DB...", "Ok");
                }
            });
        }

        private async void GoToScan(object sender, EventArgs args)
        {
            Tile t = new Tile();
            t.id = 2;
            await Navigation.PushModalAsync(new SACOScan("SCAN YOUR EMPLOYEE CARD",1, users));
        }

    }
}
