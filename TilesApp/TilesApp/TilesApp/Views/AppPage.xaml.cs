using System;
using System.Collections.Generic;
using TilesApp.Services;
using TilesApp.Rfid;
using Xamarin.Forms;
using System.Timers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TilesApp.Views
{
    public partial class AppPage : ContentPage
    {
        private int seconds = 0;
        private Timer timer = new Timer();

        private string _sessionTime;
        public string SessionTime
        {
            get
            {
                return _sessionTime;
            }
            set
            {
                if (_sessionTime != value)
                {
                    _sessionTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public AppPage()
        {
            InitializeComponent();
            BindingContext = new ConfigInfoViewModel();
            this.BindWithLifecycle(App.ViewModel.Inventory);
            NavigationPage.SetHasNavigationBar(this, false);
            
            int row = 0;
            foreach (string tag in OdooXMLRPC.userAppsList)
            {
                string[] tagArr = tag.Split('_');
                string appType = tagArr[1];
                string appName = tagArr[2];
                string icon = "";
                buttonsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                Button button = new Button
                {
                    TextColor = Color.FromHex("#ffffff"),
                    BackgroundColor = Color.FromHex("#bc0000"),
                    FontSize = 18,
                    WidthRequest = 500,
                    CornerRadius = 5,
                    FontFamily = Application.Current.Resources["CustomFont"].ToString(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    BorderColor = Color.FromHex("#796f6f"),
                    BorderWidth = 3,
                    ClassId = tag,
                    Margin = new Thickness (0, 0, 0, 20)
                };
                switch (appType)
                {
                    case "Link":
                        icon = "\ue803";
                        button.Clicked += Link_Command;
                        break;
                    case "Join":
                        icon = "\uf527";
                        button.Clicked += Join_Command;
                        break;
                    case "Reg":
                        icon = "\uf11b";
                        button.Clicked += Reg_Command;
                        break;
                    case "QC":
                        icon = "\uf14a";
                        button.Clicked += QC_Command;
                        break;
                    default:
                        break;
                }
                button.Text = appName + " " + icon;
                buttonsGrid.Children.Add(button, 0, row);
                row++;           
            }
            timer.Elapsed += OnTimerEvent;
            timer.Interval = 1000; // 1 second
            timer.Enabled = true;
        }

        private void OnTimerEvent(object sender, ElapsedEventArgs e)
        {
            seconds++;
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            SessionTime = string.Format("{0:D2} hrs {1:D2} min {2:D2} sec", t.Hours, t.Minutes, t.Seconds);
        }

        // Applications

        private async void Link_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            await Navigation.PushModalAsync(new Link(b.ClassId));
        }

        private async void Join_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            await Navigation.PushModalAsync(new Join(b.ClassId));
        }

        private async void Reg_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            await Navigation.PushModalAsync(new Reg(b.ClassId));
        }

        private async void QC_Command(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            await Navigation.PushModalAsync(new QC(b.ClassId));
        }


        // Bottom bar

        private async void Config_Command(object sender, EventArgs args)
        {
            //await DisplayAlert("CONFIGURATION", "Config...", "OK");
            await Navigation.PushModalAsync(new Configuration(this));
        }

        private async void Reader_Command(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new Rfid.Views.MainPage());
        }

        private async void Logout_Command(object sender, EventArgs args)
        {
            await DisplayAlert("You are abandoning this page", "Please, wait until Login page appears.", "OK");
            timer.Stop();
            MessagingCenter.Send(this, "OdooConnection");
            await Navigation.PopModalAsync(true);
        }

    }

    public class ConfigInfoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}