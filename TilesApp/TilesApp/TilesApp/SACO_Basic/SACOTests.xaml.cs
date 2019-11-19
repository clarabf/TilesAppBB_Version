using Xamarin.Forms;

namespace TilesApp.SACO
{

    public partial class SACOTests : ContentPage
    {
        private double width = 0;
        private double height = 0;

        public SACOTests()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            width = this.Width;
            height = this.Height;

        }

    }
}
