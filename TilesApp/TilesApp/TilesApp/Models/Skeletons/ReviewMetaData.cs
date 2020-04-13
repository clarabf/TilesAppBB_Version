using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TilesApp.Models.Skeletons
{
    public class ReviewMetaData : BaseMetaData
    {
        private Collection<string> _apps = new Collection<string>();
        [BsonIgnoreIfNull]
        public Collection<string> Apps
        {
            get
            {
                try
                {
                    if (appData[appDataIndex["Apps"]]["FieldIsSaved"])
                    {
                        JArray result = (JArray)appData[appDataIndex["Apps"]]["DefaultValue(admin)"];
                        return result.ToObject<Collection<string>>();
                    }
                }
                catch { }
                return _apps;
            }
            set
            {
                try
                {
                    appData[appDataIndex["Apps"]]["DefaultValue(admin)"] = value;
                }
                catch
                {
                    if (value !=null)
                    {
                        _apps = value;
                    }
                }
            }
        }

        //Constructor from json stream
        public ReviewMetaData(Stream streamConfig) : base(streamConfig) { }
    }
}
