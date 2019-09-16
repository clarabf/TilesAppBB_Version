using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace TilesApp
{
    public partial class TestWorkOrder : ContentPage
    {

        public class PickerItems
        {
            public string Name { get; set; }
        }

        public TestWorkOrder()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void PickerSelection(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;
            //put your code here
        }

        private async void SelectWork(object sender, EventArgs args)
        {
            Button b = (Button)sender;
            int wo;
            if (b.Text == "Work Order 1") wo = 1;
            else wo = 2;
            //CALL GetTilesOfWorkOrder(id)
            Console.WriteLine(wo);
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PopModalAsync(true);
                Navigation.PushModalAsync(new TestTiles());
            });
        }
    }
}
