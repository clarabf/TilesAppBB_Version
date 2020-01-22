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
        public bool QCPass
        {
            get
            {
                if (appData[appDataIndex["QCPass"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["QCPass"]]["DefaultValue(admin)"];
                }
                else
                {
                    return false;
                }
            }
            set
            {
                appData[appDataIndex["QCPass"]]["DefaultValue(admin)"] = value;
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
        [BsonIgnoreIfNull]
        public List<string> Images
        {
            get
            {
                if (appData[appDataIndex["Images"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["Images"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
            set {
                appData[appDataIndex["Images"]]["DefaultValue(admin)"] = value;
            }
        }

        //Constructor from json stream
        public QCMetaData(Stream streamConfig) : base(streamConfig) { }
    }
}
