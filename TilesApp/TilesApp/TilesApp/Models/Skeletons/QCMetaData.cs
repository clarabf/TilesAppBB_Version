using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using TilesApp.Services;
using Xamarin.Forms;

namespace TilesApp.Models.Skeletons
{
    public class QCMetaData : BaseData
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
        [BsonIgnore]
        private List<String> ValidCodeStructure { get; set; }
        [BsonIgnore]
        public bool? IsStationMandatory { get; set; }
        public ObservableCollection<Dictionary<string, object>> ScannerReads { get; set; } = new ObservableCollection<Dictionary<string, object>>();

        //Constructor from json string
        public QCMetaData(string jsonConfig = null) : base()
        {
            if (jsonConfig != null)
            {
                Dictionary<string, object> configData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonConfig);
                Type propertyType = GetType();

                foreach (var prop in GetType().GetProperties())
                {
                    if (configData.ContainsKey(prop.Name))
                    {
                        if (prop.Name == "AdditionalData" & configData[prop.Name] != null)
                        {
                            var content = (Dictionary<string, object>)configData[prop.Name];
                            foreach (KeyValuePair<string, object> data in content)
                            {
                                AdditionalData.Add(data.Key, data.Value);
                            }
                        }
                        else if (prop.Name == "ValidCodeStructure")
                        {
                            var content = (List<string>)configData[prop.Name];
                            foreach (string data in content)
                            {
                                ValidCodeStructure.Add(data);
                            }
                        }
                        else
                        {
                            propertyType.GetProperty(prop.Name).SetValue(this, configData[prop.Name]);
                        }
                    }
                }
            }
        }
        //Constructor from json stream
        public QCMetaData(Stream streamConfig) : base()
        {
            if (streamConfig != null)
            {
                StreamReader reader = new StreamReader(streamConfig);
                string jsonConfig = reader.ReadToEnd();
                streamConfig.Position = 0;

                Dictionary<string, object> configData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonConfig);
                Type propertyType = GetType();

                foreach (var prop in GetType().GetProperties())
                {
                    if (configData.ContainsKey(prop.Name)) { propertyType.GetProperty(prop.Name).SetValue(this, configData[prop.Name]); }
                }
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
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "Data is not a compatible JSON");
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
                        if (!(prop.Name == "Station" & IsStationMandatory == false))
                        {
                            isValid = false;
                        }
                    }
                }
            }
            return isValid;
        }
        public Boolean IsValidCode(String inputUUID)
        {
            foreach (string filterUUID in ValidCodeStructure)
            {
                Boolean isValidCode = true;

                if (System.Text.RegularExpressions.Regex.IsMatch(inputUUID, @"\A\b[0-9a-fA-F]+\b\Z") & inputUUID.Length == 24 & System.Text.RegularExpressions.Regex.IsMatch(filterUUID, @"\A\b[0-9a-fA-FX]+\b\Z") & filterUUID.Length == 24)
                {
                    for (int i = 0; i < inputUUID.Length / 2; i++)
                    {
                        if (filterUUID.Substring(i * 2, 2) != "XX" && inputUUID.Substring(i * 2, 2) != filterUUID.Substring(i * 2, 2))
                        {
                            isValidCode = false;
                        }
                    }
                    if (isValidCode) { return true; }
                }
            }
            return false;
        }
        //CHECK PROCESS INPUT
        public void ProcessInput(Dictionary<string, object> input)
        {
            //First check if it follows config file code connvention (Aka ValidCodeStructure)
            if (IsValidCode(input[nameof(InputDataProps.Value)].ToString()))
            {
                //Now see if already on list
                foreach (var item in ScannerReads.ToList())
                {
                    if (item[nameof(InputDataProps.Value)].ToString() == input[nameof(InputDataProps.Value)].ToString())
                    {
                        return;
                    }
                }
                ScannerReads.Add(input);
            }
        }
    }
}
