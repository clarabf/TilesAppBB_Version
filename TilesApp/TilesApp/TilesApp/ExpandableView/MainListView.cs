using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TilesApp.ExpandableView
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainListView : ContentView
	{
        private Product _oldProduct;
        public ObservableCollection<Product> Products { get; set; }



        public MainListView()
        {
            Products = new ObservableCollection<Product>
            {
                new Product
                {
                    Title = "Microsoft Surface",

                    IsVisible =false
                },
                new Product
                {
                    Title = "Microsoft Lumia 535",
                    IsVisible = false
                },
                new Product
                {
                    Title = "Microsoft 650",
                    IsVisible = false
                },
                new Product
                {
                    Title = "Lenovo Surface",
                    IsVisible =  false
                },
                new Product
                {
                    Title = "Microsoft edge",
                    IsVisible = false
                }
            };




        }
        public void ShoworHiddenProducts(Product product)
        {
            if (_oldProduct == product)
            {
                product.IsVisible = !product.IsVisible;
                UpDateProducts(product);
            }
            else
            {
                if (_oldProduct != null)
                {
                    _oldProduct.IsVisible = false;
                    UpDateProducts(_oldProduct);

                }
                product.IsVisible = true;
                UpDateProducts(product);
            }
            _oldProduct = product;
        }

        private void UpDateProducts(Product product)
        {

            var Index = Products.IndexOf(product);
            Products.Remove(product);
            Products.Insert(Index, product);

        }
    }

   
}