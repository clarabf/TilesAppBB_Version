using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using TilesApp.Azure;

namespace TilesApp.Models.Skeletons
{
    public class QCMetaData
    {
        public bool? QCPass { get; set; }
        public String QCProcedureDetails { get; set; }
        [BsonIgnoreIfNull]
        public String QCResultDetails { get; set; }
        [BsonIgnoreIfNull]
        public Dictionary<String, String> QCAttachedFilesHASHnURL { get; set; }
        [BsonIgnoreIfNull]
        public Dictionary<string, object> AdditionalData { get; set; }
        [BsonIgnore]
        public bool AreQCResultDetailsMandatory { get; set; }
        [BsonIgnore]
        public bool AreAttachedFilesMandatory { get; set; }

        //Constructor from json string
        public QCMetaData(string jsonConfig = "")
        {
            Dictionary<string, object> configData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonConfig);
            Type propertyType = GetType();

            foreach (var prop in GetType().GetProperties())
            {
                propertyType.GetProperty(prop.Name).SetValue(this, configData[prop.Name]);
            }
        }
        //Constructor from json stream
        public QCMetaData(Stream streamConfig)
        {
            StreamReader reader = new StreamReader(streamConfig);
            string jsonConfig = reader.ReadToEnd();

            Dictionary<string, object> configData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonConfig);
            Type propertyType = GetType();

            foreach (var prop in GetType().GetProperties())
            {
                propertyType.GetProperty(prop.Name).SetValue(this, configData[prop.Name]);
            }
        }
        //Add metadata from string
        public void AddQRMetaData(string jsonConfig)
        {
            try
            {
                Dictionary<string, object> configData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonConfig);
                Type propertyType = GetType();

                foreach (var prop in GetType().GetProperties())
                {
                    if (prop.Name != "AdditionalData" & prop.Name != "AreQCResultDetailsMandatory" & prop.Name != "AreAttachedFilesMandatory")
                    {
                        propertyType.GetProperty(prop.Name).SetValue(this, configData[prop.Name]);
                    }
                    else if (prop.Name == "AdditionalData" & prop.Name != "AreQCResultDetailsMandatory" & prop.Name != "AreAttachedFilesMandatory")
                    {
                        foreach (var key in configData.Keys)
                        {
                            if (!AdditionalData.ContainsKey(key))
                            {
                                AdditionalData.Add(key, configData[key]);
                            }
                            else
                            {
                                AdditionalData[key] = configData[key];
                            }
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("Data is not a JSON");
            }
        }
        public void AddAttachedFile(Stream fileStream, String appName)
        {
            Dictionary<string, string> hashAndURL = StreamToAzure.WriteJPEGStream(fileStream, appName);
            QCAttachedFilesHASHnURL.Add(hashAndURL["ContentMD5"], hashAndURL["Uri"]);
        }
        public void AddAttachedFiles(List<Stream> fileStream, String appName)
        {
            List<Dictionary<string, string>> hashAndURLs = StreamToAzure.WriteJPEGStreams(fileStream, appName);

            foreach (Dictionary<string, string> hashAndURL in hashAndURLs)
            {
                QCAttachedFilesHASHnURL.Add(hashAndURL["ContentMD5"], hashAndURL["Uri"]);
            }
        }
        public Boolean IsValid()
        {
            var isValid = true;

            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.GetValue(this, null) == null & prop.Name != "AdditionalData")
                {
                    if ((prop.Name == "QCResultDetails" & AreQCResultDetailsMandatory == true) || (prop.Name == "QCAttachedFilesHASHnURL" & AreAttachedFilesMandatory == true))
                    {
                        isValid = false;
                    }
                }
            }
            return isValid;
        }
    }
}
