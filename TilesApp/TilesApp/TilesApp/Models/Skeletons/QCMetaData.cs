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
        private bool _qcPass;
        private Collection<string> _images = new Collection<string>();
       //Fields properties
       [BsonIgnoreIfNull]
        public bool QCPass
        {
            get
            {
                try
                {
                    if (appData[appDataIndex["QCPass"]]["FieldIsSaved"])
                    {
                        return appData[appDataIndex["QCPass"]]["DefaultValue(admin)"];
                    }
                }
                catch                
                {
                }
                return _qcPass;
            }
            set
            {
                try
                {
                    appData[appDataIndex["QCPass"]]["DefaultValue(admin)"] = value;
                }
                catch
                {
                    _qcPass = value;
                }
            }
        }
        [BsonIgnoreIfNull]
        public string QCProcedureDetails
        {
            get
            {
                try
                {
                    if (appData[appDataIndex["QCProcedureDetails"]]["FieldIsSaved"])
                    {
                        return appData[appDataIndex["QCProcedureDetails"]]["DefaultValue(admin)"];
                    }
                }
                catch { }
                return null;
            }
        }
        [BsonIgnoreIfNull]
        public string QCResultDetails
        {
            get
            {
                try
                {
                    if (appData[appDataIndex["QCResultDetails"]]["FieldIsSaved"])
                    {
                        return appData[appDataIndex["QCResultDetails"]]["DefaultValue(admin)"];
                    }
                }
                catch{}
                return null;
            }
        }
        [BsonIgnoreIfNull]
        public Collection<string> Images
        {
            get
            {
                try
                {
                    if (appData[appDataIndex["Images"]]["FieldIsSaved"])
                    {
                        return appData[appDataIndex["Images"]]["DefaultValue(admin)"];
                    }
                }
                catch 
                {
                }
                return _images;
            }
            set
            {
                try
                {
                    appData[appDataIndex["Images"]]["DefaultValue(admin)"] = value;
                }
                catch
                {
                    if(value !=null)_images = value;
                }
            }
        }

        //Constructor from json stream
        public QCMetaData(Stream streamConfig) : base(streamConfig) { }
    }
}
