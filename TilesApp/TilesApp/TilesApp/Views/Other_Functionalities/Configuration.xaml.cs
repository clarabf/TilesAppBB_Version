using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesApp.Models;
using TilesApp.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Permissions.Abstractions;

namespace TilesApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Configuration : ContentPage
    {
        public Configuration(AppPage appPage)
        {
            InitializeComponent();
            BindingContext = appPage;
            GetDeviceLocation();
            App.Inventory.Clear();

            lblName.Text = OdooXMLRPC.userName;
            if (App.Station != null)
            {
                lblStation.Text = "Station: " + App.Station + "\nYou can change it by scanning again:";
                btAdd.Text = "CHANGE \uf0ec";
            }
            else
            {
                lblStation.Text = "You haven't assigned a station. Please scan one:";
            }
            MessagingCenter.Subscribe<Scan, string>(this, "SetStation", (s, qrContent) => {
                lblStation.Text = "Station: " + qrContent + "\nYou can change it by scanning again:";
                btAdd.Text = "CHANGE \uf0ec";
                App.Station = qrContent;
            });
        }

        private async void GetDeviceLocation()
        {
            Xamarin.Essentials.Location rowGeoLocation = new Xamarin.Essentials.Location();
            Models.Location location = new Models.Location();
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    /*
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await DisplayAlert("Location Permission", "We need to access your location", "OK");
                    }
                    */
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {
                    rowGeoLocation = Geolocation.GetLastKnownLocationAsync().Result;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                }

            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch
            {
                return;
            }
            if (rowGeoLocation != null)
            {
                if (App.GeoLocation != null)
                {
                    if (!App.GeoLocation.lat.Equals(rowGeoLocation.Latitude.ToString()) || !App.GeoLocation.lon.Equals(rowGeoLocation.Longitude.ToString()))
                    {
                        location = App.GeoLocation = await HttpClientManager.ReverseGeoCodeAsync(rowGeoLocation.Latitude.ToString(), rowGeoLocation.Longitude.ToString());
                        lblLocation.Text = location.address["city"] + ", " + location.address["state"] + ", " + location.address["country"];
                    }
                }
                else {
                    location = App.GeoLocation = await HttpClientManager.ReverseGeoCodeAsync(rowGeoLocation.Latitude.ToString(), rowGeoLocation.Longitude.ToString());
                    lblLocation.Text = location.address["city"] + ", " + location.address["state"] + ", " + location.address["country"];
                }

            }
        }

        private async void GoToScan(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Scan("SCAN YOUR STATION", 3, null));
        }

        private async void Home(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

        // OVERRIDES
        protected override void OnAppearing()
        {
            App.Inventory.CollectionChanged += Inventory_CollectionChanged;
            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            // UNSUBSRCIBE WHEN PAGE IS CLOSED
            //Unsubscribe();
            App.Inventory.CollectionChanged -= Inventory_CollectionChanged;
            base.OnDisappearing();
        }
        private void Inventory_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (var InputWithDevice in args.NewItems.Cast<Dictionary<string, object>>())
                {
                    lblStation.Text = "Station: " + (string)InputWithDevice["Value"] + "\nYou can change it by scanning again:";
                    btAdd.Text = "CHANGE \uf0ec";
                    App.Station = (string)InputWithDevice["Value"];
                }
            }
        }
    }
}