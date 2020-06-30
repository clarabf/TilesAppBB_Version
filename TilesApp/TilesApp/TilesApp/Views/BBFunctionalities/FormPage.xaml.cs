using System;
using TilesApp.Services;
using TilesApp.Rfid;
using Xamarin.Forms;
using System.Timers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TilesApp.Models;
using TilesApp.Models.DataModels;
using System.Collections.Generic;
using TilesApp.Views.Other_Functionalities;
using Xamarin.Essentials;
using Android.Content;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.Toast;

namespace TilesApp.Views
{
    public partial class FormPage : ContentPage
    {
        public FormPage(string title, List<Web_Field> formFields)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            int row = 0;
            lblTitle.Text = title.ToUpper();
            foreach (Web_Field field in formFields)
            {
                elementsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                ExtendedButton button = new ExtendedButton
                {
                    TextColor = Color.FromHex("#ffffff"),
                    BackgroundColor = Color.FromHex("#bc0000"),
                    FontSize = 18,
                    WidthRequest = 500,
                    CornerRadius = 5,
                    FontFamily = Application.Current.Resources["CustomFont"].ToString(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    BorderColor = Color.FromHex("#796f6f"),
                    BorderWidth = 3,
                    HorizontalTextAlignment = TextAlignment.Start,
                    ClassId = field.Name,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                button.Text = field.LongName;
                button.Padding = new Thickness(10, 15, 0, 0);
                elementsGrid.Children.Add(button, 0, row);
                row++;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopModalAsync(true);
            Navigation.PushModalAsync(new FamilyAndGroups());
            return true;
        }
    }

}