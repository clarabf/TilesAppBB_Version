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
                Label lblElement = new Label
                {
                    Text = field.LongName,
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    FontSize = 18,
                };
                elementsGrid.Children.Add(lblElement, 0, row);
                row++;
                switch (field.PrimitiveType)
                {
                    //integer
                    case 0:
                        if (field.PrimitiveQuantity == 1)
                        {
                            Picker picker = new Picker
                            {
                                ClassId = field.CosmoId,
                                BackgroundColor = Color.White,
                                Title = field.LongName,
                                VerticalOptions = LayoutOptions.StartAndExpand
                            };
                            picker.Items.Add("True");
                            picker.Items.Add("False");
                            elementsGrid.Children.Add(picker, 0, row);
                            row++;
                        }
                        break;
                    case 1:
                        break;
                    //string
                    case 2:
                        Entry entry = new Entry {
                            ClassId = field.CosmoId,
                            BackgroundColor = Color.White,
                            Placeholder = field.LongName + " (max. " + field.PrimitiveQuantity + ")", 
                            VerticalOptions = LayoutOptions.StartAndExpand
                        };
                        elementsGrid.Children.Add(entry, 0, row);
                        row++;
                        break;
                }
            }
            for (int i = row; i < 12; i++)
            {
                elementsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
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