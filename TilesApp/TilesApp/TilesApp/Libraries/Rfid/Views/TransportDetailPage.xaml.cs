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

        private async void Cancel(object sender, EventArgs args)
        {
            await Navigation.PopAsync(true);
            this.OnBackButtonPressed();
        }

        private void ConnectDevice(object sender, EventArgs args)
        {
            Button connectBtn = (Button)sender;
            if (this.ViewModel.DisplayName.Contains("1128"))
            {
                connectBtn.SetBinding(Button.CommandProperty, new Binding() { Source = ViewModel, Path = "ConnectCommand" });
                ViewModel.ConnectCommand.Execute(null);
            }
            else
            {
                App.ViewModel.Transports.AddNewCommand.Execute(null);
            }            
        }
    }
}