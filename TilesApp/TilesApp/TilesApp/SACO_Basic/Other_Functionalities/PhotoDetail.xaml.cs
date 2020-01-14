using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TilesApp.SACO
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoDetail : ContentPage
    {

        public ObservableCollection<PhotoData> TakenPhotos { get; set; } = new ObservableCollection<PhotoData>();

        public PhotoDetail(ObservableCollection<PhotoData> takenPhotos)
        {
            InitializeComponent();
            BindingContext = this;
            foreach (PhotoData phD in takenPhotos)
            {
                TakenPhotos.Add(phD);
            }
            //TakenPhotos.Add(new PhotoData() { Path = "lalalalala", Time = DateTime.Now.ToShortTimeString(), ImageSource = "delete.png" });
        }

        private async void Cancel(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync(true);
        }

        private async void Delete_Photo(object sender, EventArgs args)
        {
            TakenPhotos.Add(new PhotoData() { Path = "lelelelel", Time = DateTime.Now.ToShortTimeString(), ImageSource = "delete.png" });
            MessagingCenter.Send(this, "SendPhotos", TakenPhotos);
            await DisplayAlert("Delete photo", "Photo has been successfully deleted!", "Ok");
        }

        //public void OnMore(object sender, EventArgs e)
        //{
        //    var mi = ((MenuItem)sender);
        //    DisplayAlert("More Context Action", mi.CommandParameter + " more context action", "OK");
        //}

        //public void OnDelete(object sender, EventArgs e)
        //{
        //    var mi = ((MenuItem)sender);
        //    DisplayAlert("Delete Context Action", mi.CommandParameter + " delete context action", "OK");
        //}
    }
}