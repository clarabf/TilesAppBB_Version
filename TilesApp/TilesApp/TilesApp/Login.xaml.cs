using System;
using TilesApp.Models;
using Xamarin.Forms;

namespace TilesApp
{

    public partial class Login : ContentPage
    {
        private double width = 0;
        private double height = 0;
        public Login()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;

        }

        private async void GoToScan(object sender, EventArgs args)
        {
            Tile t = new Tile();
            t.id = 2;
            await Navigation.PushModalAsync(new ScanQR(t));
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width != this.width || height != this.height)
            {
                this.width = width;
                this.height = height;
                if (width > height)
                {
                   
                    innerGrid.ColumnDefinitions.Clear();
              
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.7, GridUnitType.Star) });
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.95, GridUnitType.Star) });

                }
                else
                {
                    innerGrid.ColumnDefinitions.Clear();
                    innerGrid.ColumnSpacing = 12;
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.05, GridUnitType.Star) });
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.1, GridUnitType.Star) });
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.1, GridUnitType.Star) });
                    innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.01, GridUnitType.Star) });

                }
            }
        }

    }
}
