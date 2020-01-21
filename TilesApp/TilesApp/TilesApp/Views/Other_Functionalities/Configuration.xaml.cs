using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TilesApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Configuration : ContentPage
    {
        public Configuration(AppPage appPage)
        {
            InitializeComponent();
            BindingContext = appPage;
            lblName.Text = OdooXMLRPC.userName;
            lblId.Text = OdooXMLRPC.userID.ToString();
            if (App.Station!=null)
            {
                lblStation.Text = "Station: " + App.Station + "\nYou can change it by scanning again:";
                btAdd.Text = "CHANGE";
            }
            else
            {
                lblStation.Text = "You haven't assigned a station. Please scan one:";
            }
            MessagingCenter.Subscribe<Scan, string>(this, "SetStation", (s, qrContent) => {
                lblStation.Text = "Station: " + qrContent + "\nYou can change it by scanning again:";
                btAdd.Text = "CHANGE";
                App.Station = qrContent;
            });
        }

        private async void GoToScan(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Scan("SCAN YOUR STATION", 3, null));
        }

        private async void Home(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}