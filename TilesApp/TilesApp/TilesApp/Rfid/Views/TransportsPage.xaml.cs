using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using TilesApp.Rfid.Models;
using TilesApp.Rfid.Views;
using TilesApp.Rfid.ViewModels;

namespace TilesApp.Rfid.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransportsPage : ContentPage
    {
        TransportsViewModel viewModel;

        public TransportsPage()
        {
            InitializeComponent();

            this.BindWithLifecycle(viewModel = App.ViewModel.Transports);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var transportModel = args.SelectedItem as ViewModels.TransportViewModel;
            if (transportModel != null)
            {
                await Navigation.PushAsync(new TransportDetailPage() { ViewModel = transportModel });

                // deselect for next time
                this.TransportsListView.SelectedItem = null;
            }
        }
    }
}