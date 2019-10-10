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

        public TestTableOrder()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

    }
}
