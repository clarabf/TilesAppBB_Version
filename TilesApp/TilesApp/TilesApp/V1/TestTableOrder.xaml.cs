using System;
using Xamarin.Forms;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using TilesApp.Models;
using TilesApp.ExpandableView;

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
            BindingContext = new ListViewPageModel();
        }

        private async void GoToStep(object sender, EventArgs args)
        {
            Tile t = new Tile(); t.id = 2;
            await Navigation.PushModalAsync(new StepsPage(t, 2, 5, "user", "http://oboria.net/docs/pdf/ftp/2/3.PDF",3));
        }

      /*  private void Collapsed(object sender, EventArgs e)
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
            title.FontSize = 9;
            grid.Children.Add(title, 0, 0);
            Grid.SetColumnSpan(title, 4);
            Label Ref = new Label
            {
                Text = "SACO Product Ref. Nr.",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            };
            grid.Children.Add(Ref, 0, 1);
            Ref.FontSize = 8;
            Label Sales = new Label
            {
                Text = "SACO Product Sales R",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            };
            grid.Children.Add(Sales, 0, 2);
            Sales.FontSize = 8;
            Label Manufacture = new Label
            {
                Text = "Qty. to Manufacture",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            };
            grid.Children.Add(Manufacture, 2, 1);
            Manufacture.FontSize = 8;
            Label Steps = new Label
            {
                Text = "Steps:",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            };
            grid.Children.Add(Steps, 2, 2);
            Steps.FontSize = 8;
            Label Value1 = new Label
            {
                Text = "23412",
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            };
            grid.Children.Add(Value1, 1, 1);
            Value1.FontSize = 8;
            Label Value2 = new Label
            {
                Text = "Tile Type 00000/00000/03/00/00/00/00000/00000",
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            };
            grid.Children.Add(Value2, 1, 2);
            Value2.FontSize = 9;
            Label Value3 = new Label
            {
                Text = "16 u",
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.StartAndExpand
            };
            grid.Children.Add(Value3, 3, 1);
            Value3.FontSize = 8;
            Label Value4 = new Label
            {
                Text = "1 to",
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand
            };
            grid.Children.Add(Value4, 3, 2);
            Value4.FontSize = 8;
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
        }*/

    }
}