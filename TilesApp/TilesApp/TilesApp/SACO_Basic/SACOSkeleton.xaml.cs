using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOSkeleton : ContentPage
    {

        public SACOSkeleton()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
