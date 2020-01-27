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
    public class RegMetaData : BaseMetaData
    {
        //Fields properties
        [BsonIgnoreIfNull]
        public string Registry
        {
            get
            {
                if (appData[appDataIndex["Registry"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["Registry"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        [BsonIgnoreIfNull]
        public string RegistryDetails
        {
            get
            {
                if (appData[appDataIndex["RegistryDetails"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["RegistryDetails"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }        

        //Constructor from json stream
        public RegMetaData(Stream streamConfig) : base(streamConfig) { }
    }
}
