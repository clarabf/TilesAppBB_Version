using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace TilesApp.Models
{
    public class PhotoData
    {

        public string FileName { get; set; }
        public string DateAndTime { get; set; }
        public string ImageSource { get; set; }

        public Stream FileContent;
    }
}
