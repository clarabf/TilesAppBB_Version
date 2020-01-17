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
        //Fields properties
        [BsonIgnoreIfNull]
        public string ProductFamily
        {
            get
            {
                if (appData[appDataIndex["ProductFamily"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["ProductFamily"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        [BsonIgnoreIfNull]
        public string ProductType
        {
            get
            {
                if (appData[appDataIndex["ProductType"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["ProductType"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        [BsonIgnoreIfNull]
        public string ProductModel
        {
            get
            {
                if (appData[appDataIndex["ProductModel"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["ProductModel"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        [BsonIgnoreIfNull]
        public string ProductGeneration
        {
            get
            {
                if (appData[appDataIndex["ProductGeneration"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["ProductGeneration"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        [BsonIgnoreIfNull]
        public string ProductPartNr
        {
            get
            {
                if (appData[appDataIndex["ProductPartNr"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["ProductPartNr"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        [BsonIgnoreIfNull]
        public string ProductInfo
        {
            get
            {
                if (appData[appDataIndex["ProductInfo"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["ProductInfo"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        [BsonIgnoreIfNull]
        public string ProductCatalogueURL
        {
            get
            {
                if (appData[appDataIndex["ProductCatalogueURL"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["ProductCatalogueURL"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        [BsonIgnoreIfNull]
        public string ProductManualURL
        {
            get
            {
                if (appData[appDataIndex["ProductManualURL"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["ProductManualURL"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }

        //Constructor from json stream
        public LinkMetaData(Stream streamConfig) : base(streamConfig) { }
    }
}
