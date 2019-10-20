using System;
using Xamarin.Forms;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using TilesApp.Models;

namespace TilesApp.V1
{
    public partial class TestTableOrder : ContentPage
    {
        private Boolean InfoRow = false;

        public TestTableOrder(string user_name)
        {
            InitializeComponent();
            user.Text = user_name;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void GoToStep(object sender, EventArgs args)
        {
            Tile t = new Tile(); t.id = 2;
            await Navigation.PushModalAsync(new StepsPage(t, 2, 5, "user", "http://oboria.net/docs/pdf/ftp/2/3.PDF",3));
        }

        private void Collapsed(object sender, EventArgs e)
        {
            var table = new TableSection();
            var layout = new StackLayout() { Orientation = StackOrientation.Horizontal };

            Grid grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                    },
                ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2.5, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) },
                    }
            };
            grid.BackgroundColor = Color.FromHex("#fbfbfb");
            Label title = new Label
            {
                Text = "WORK ORDER ADDITIONAL DETAILS",
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.CenterAndExpand

            };
            grid.Children.Add(title, 0, 0);
            Grid.SetColumnSpan(title, 4);
            grid.Children.Add(new Label
            {
                Text = "SACO Product Ref. Nr.",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            }, 0, 1);

            grid.Children.Add(new Label
            {
                Text = "SACO Product Sales R",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            }, 0, 2);

            grid.Children.Add(new Label
            {
                Text = "Qty. to Manufacture",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            }, 2, 1);
            grid.Children.Add(new Label
            {
                Text = "Steps:",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            }, 2, 2);
            grid.Children.Add(new Label
            {
                Text = "23412",
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            }, 1, 1);

            grid.Children.Add(new Label
            {
                Text = "Tile Type 00000/00000/03/00/00/00/00000/00000",
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            }, 1, 2);
            grid.Children.Add(new Label
            {
                Text = "16 u",
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            }, 3, 1);
            grid.Children.Add(new Label
            {
                Text = "1 to",
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            }, 3, 2);
            layout.Children.Add(grid);
            // Adding first row of the table

            table.Add(new ViewCell() { View = layout });

            // Scan the tiles table            
            layout.ClassId = "MoreInfo";
            if (InfoRow)
            {
                layout.IsVisible = false;
              //  StackLayout.HeightRequest = 0;
            }
            else
            {
                layout.IsVisible = true;
                InfoRow = true;
                TableRoot.Add(table);
            }
        }

    }
}