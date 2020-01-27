using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace TilesApp.Models.Skeletons
{
    public class LinkMetaData : BaseMetaData
    {
        //No Fields properties

        //Constructor from json stream
        public LinkMetaData(Stream streamConfig) : base(streamConfig) { }
    }
}
