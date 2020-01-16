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
        public string Operation
        {
            get
            {
                if (appData[appDataIndex["Operation"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["Operation"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        [BsonIgnoreIfNull]
        public string OperationDetails
        {
            get
            {
                if (appData[appDataIndex["OperationDetails"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["OperationDetails"]]["DefaultValue(admin)"];
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
