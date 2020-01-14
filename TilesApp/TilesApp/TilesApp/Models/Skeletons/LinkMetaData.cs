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
    public class LinkMetaData : BaseData
    {
        public String ProductFamily { get; set; }
        public String ProductType { get; set; }
        public String ProductModel { get; set; }
        public String ProductGeneration { get; set; }
        public String ProductPartNr { get; set; }
        [BsonIgnoreIfNull]
        public String ProductInfo { get; set; }
        [BsonIgnoreIfNull]
        public String ProductCatalogueURL { get; set; }
        [BsonIgnoreIfNull]
        public String ProductManualURL { get; set; }
        [BsonIgnoreIfNull]
        public Dictionary<string, object> AdditionalData { get; set; }
        [BsonIgnore]
        private List<String> ValidCodeStructure { get; set; }
        [BsonIgnore]
        public bool? IsStationMandatory { get; set; }
        //CHECK THE BEST WAY TO SAVE THE COLLECTION TO BSON
        public ObservableCollection<Dictionary<string, object>> ScannerReads { get; set; } = new ObservableCollection<Dictionary<string, object>>();

        //Constructor from json string
        public LinkMetaData(string jsonConfig = null) : base()
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
        public LinkMetaData(Stream streamConfig) : base()
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
                throw new Exception("Data is not a JSON");
            }
        }
        public Boolean IsValid()
        {
            //CHECK VALID "FALSE". IT IS PROBABLY ADDITIONAL DATA DICTIONARY THAT FAILS.
            var isValid = true;

            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.GetValue(this, null) == null & prop.Name != "AdditionalData" & prop.Name != "ProductManualURL" & prop.Name != "ProductCatalogueURL" & prop.Name != "ProductInfo")
                {
                    if (!(prop.Name == "Station" & IsStationMandatory == false))
                    {
                        isValid = false;
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
        public void ProcessInput(string code, Enum reader)
        {
            //First check if it follows config file code connvention (Aka ValidCodeStructure)
            if (IsValidCode(code))
            {
                //Now see if already on list
                foreach (var item in ScannerReads.ToList())
                {
                    if (item[nameof(InputDataProps.Value)].ToString() == code)
                    {
                        return;
                    }
                }
                Dictionary<string, object> input = new Dictionary<string, object>();
                input.Add(nameof(InputDataProps.Value), code);
                input.Add(nameof(InputDataProps.ReaderType), reader);
                input.Add(nameof(InputDataProps.Timestamp), DateTime.Now);
                ScannerReads.Add(input);
            }
        }
    }
}
