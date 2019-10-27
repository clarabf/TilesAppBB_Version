using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TilesApp.ExpandableView
{
    public partial class MainPage1 : ContentPage
    {
        public MainPage1()
        {
            InitializeComponent();
        }
        private void ListViewItem_Tabbed(object sender, ItemTappedEventArgs e)
        {
            var product = e.Item as Product;
            var vm = BindingContext as MainListView;
            vm?.ShoworHiddenProducts(product);
        }

    }
}
