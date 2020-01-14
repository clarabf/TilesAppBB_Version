using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace TilesApp.Models
{
    public class PhotoData
    {

        public string Path { get; set; }
        public string Time { get; set; }
        public string ImageSource { get; set; }


        //public ObservableCollection<PhotoInfo> TakenPhotos { get; set; } = new ObservableCollection<PhotoInfo>();
        //public int NumTakenPhotos
        //{
        //    get { return TakenPhotos.Count; }
        //}

        //public PhotoData()
        //{
        //TakenPhotos = new ObservableCollection<PhotoInfo>
        //{
        //    new PhotoInfo(){ Path = "image_1.jpg", Time = "12/04/2019", ImageSource = "delete.png" },
        //    new PhotoInfo(){ Path = "image_2.png", Time = "21/12/2019", ImageSource = "delete.png" },
        //    new PhotoInfo(){ Path = "image_3.jpeg", Time = "01/01/2020", ImageSource = "delete.png" }
        //};
        //}
    }
}
