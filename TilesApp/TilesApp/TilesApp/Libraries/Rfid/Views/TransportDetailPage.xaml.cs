using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using TilesApp.Rfid.Models;
using TilesApp.Rfid.ViewModels;

namespace TilesApp.Rfid.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransportDetailPage : ContentPage
    {
        TransportViewModel viewModel;

        //public TransportDetailPage(TransportViewModel viewModel)
        //{
        //    InitializeComponent();

        //    BindingContext = this.viewModel = viewModel;
        //}

        public TransportViewModel ViewModel
        {
            get => this.viewModel;
            set => this.BindingContext = this.viewModel = value;
        }
            

        public TransportDetailPage()
        {
            InitializeComponent();


        }

        private void ConnectBtn_Clicked(object sender, EventArgs e)
        {
            App.ViewModel.Transports.AddNewCommand.Execute(null);
        }

        private async void Cancel(object sender, EventArgs args)
        {
            await Navigation.PopAsync(true);
            this.OnBackButtonPressed();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (this.ViewModel.DisplayName.Contains("1128"))
            {
                ConnectBtn.SetBinding(Button.CommandProperty, new Binding() { Source = ViewModel, Path = "ConnectCommand" });
            }
            else
            {
                ConnectBtn.Clicked += ConnectBtn_Clicked;
            }
        }
    }
}