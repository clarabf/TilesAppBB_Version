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
        }

        private void Delete_Photo(object sender, EventArgs args)
        {
            ImageButton b = (ImageButton)sender;
            PhotoData phD = TakenPhotos.FirstOrDefault(ph => ph.FileName == b.ClassId);
            TakenPhotos.Remove(phD);
        }
        private async void Cancel(object sender, EventArgs args)
        {
            await DisplayAlert("Edition complete", "No changes have been made.", "Ok");
            await Navigation.PopModalAsync(true);
        }

        private async void SaveAndFinish(object sender, EventArgs args)
        {
            MessagingCenter.Send(this, "SendPhotos", TakenPhotos);
            await DisplayAlert("Edition complete", "Photos have been successfully deleted!", "Ok");
            await Navigation.PopModalAsync(true);
        }

    }
}