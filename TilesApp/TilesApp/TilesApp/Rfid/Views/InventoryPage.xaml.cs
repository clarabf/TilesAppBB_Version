using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TilesApp.Rfid.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryPage : ContentPage
    {
        public InventoryPage()
        {
            InitializeComponent();

            this.BindWithLifecycle(App.ViewModel.Inventory);
        }

        private string SelectedTransponder { get; set; }      

        /*async*/ void ReadWrite_Clicked(object sender, EventArgs e)
        {
            if (this.SelectedTransponder != null)
            {
                //var page = new ReadWritePage();// App.Locator.Locate<ReadWritePage>();
                //page.ViewModel.HexIdentifier = this.SelectedTransponder;
                //await Navigation.PushModalAsync(page);
                App.ViewModel.ReadWrite.HexIdentifier = this.SelectedTransponder;
                App.ShowReadWrite();
            }
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            //var model = this.BindingContext as ViewModels.TransportsViewModel;
            var item = args.SelectedItem as Models.IdentifiedItem;
            if (item != null)
            {
                this.SelectedTransponder = item.Identifier;
            }
        }
    }
}