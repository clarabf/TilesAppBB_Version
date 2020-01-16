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
    public class QCMetaData : BaseMetaData
    {
        //Fields properties
        [BsonIgnoreIfNull]
        public string QCPass
        {
            get
            {
                if (appData[appDataIndex["QCPass"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["QCPass"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        [BsonIgnoreIfNull]
        public string QCProcedureDetails
        {
            get
            {
                if (appData[appDataIndex["QCProcedureDetails"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["QCProcedureDetails"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        [BsonIgnoreIfNull]
        public string QCResultDetails
        {
            get
            {
                if (appData[appDataIndex["QCResultDetails"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["QCResultDetails"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }

        //Constructor from json stream
        public QCMetaData(Stream streamConfig) : base(streamConfig) { }
    }
}
