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
    public partial class ReadersPage : BasePage
    {
        ReadersViewModel viewModel;

        public ReadersPage()
        {
            InitializeComponent();

            //this.BindWithLifecycle(viewModel = App.Locator.Locate<ReadersViewModel>());
            this.BindingContext = this.viewModel = App.ViewModel.Readers;
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            this.viewModel.SelectedReader =  args.SelectedItem as ReaderViewModel;
        }
    }
}