using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TilesApp.Rfid.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReadTestPage : ContentPage
    {
        public ReadTestPage()
        {
            InitializeComponent();

            this.ViewModel = App.ViewModel.ReadWrite;
            this.BindWithLifecycle(this.ViewModel);

        }

        public ViewModels.ReadWriteViewModel ViewModel { get; set; }
    }
}