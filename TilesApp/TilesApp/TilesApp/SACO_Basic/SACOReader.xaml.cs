using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOReader : ContentPage
    {
        private double width = 0;
        private double height = 0;

        public SACOReader()
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;
            MessagingCenter.Subscribe<Application, String>(Application.Current, "SendBarcode", (s, a) => {
                label.Text = "... " + a.ToString() + " ...";
            });
        }

    }
}
