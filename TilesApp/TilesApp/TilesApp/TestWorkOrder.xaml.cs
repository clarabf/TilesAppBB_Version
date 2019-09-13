using System;
using Xamarin.Forms;

namespace TilesApp
{
    public partial class TestWorkOrder : ContentPage
    {
        public TestWorkOrder()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void SelectWork(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            int wo;
            if (b.Text == "Work Order 1") wo = 1;
            else wo = 2;
            //CALL GetTilesFuncion
            Console.WriteLine(wo);
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PopModalAsync(true);
                Navigation.PushModalAsync(new TestTiles());
            });
        }
    }
}
