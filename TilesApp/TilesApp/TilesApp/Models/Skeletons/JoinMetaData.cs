using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace TilesApp.Models.Skeletons
{
    public class JoinMetaData
    {
        public String ParentUUID { get; set; }
        [BsonIgnore]
        public String ParentUUIDStructure { get; set; }
        [BsonIgnoreIfNull]
        private Dictionary<string, object> AdditionalData { get; set; }

        //Constructor from json string
        public JoinMetaData(string jsonConfig = null)
        {
            if (jsonConfig != null)
            {
                Dictionary<string, object> configData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonConfig);
                Type propertyType = GetType();

                foreach (var prop in GetType().GetProperties())
                {
                    if (configData.ContainsKey(prop.Name)) { propertyType.GetProperty(prop.Name).SetValue(this, configData[prop.Name]); }
                }
            }
        }
        //Constructor from json stream
        public JoinMetaData(Stream streamConfig)
        {
            if (streamConfig != null)
            {
                StreamReader reader = new StreamReader(streamConfig);
                string jsonConfig = reader.ReadToEnd();

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
                throw new Exception("Data is not a compatible JSON");
            }
        }
        public Boolean IsValid()
        {
            var isValid = true;

            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.GetValue(this, null) == null & prop.Name != "AdditionalData")
                {
                    isValid = false;
                }
            }
            return isValid;
        }
        public Boolean IsParent(String inputUUID)
        {
            var isParent = true;

            if (System.Text.RegularExpressions.Regex.IsMatch(inputUUID, @"\A\b[0-9a-fA-F]+\b\Z") & inputUUID.Length == 24 & System.Text.RegularExpressions.Regex.IsMatch(ParentUUIDStructure, @"\A\b[0-9a-fA-FX]+\b\Z") & ParentUUIDStructure.Length == 24)
            {
                for (int i = 0; i < inputUUID.Length / 2; i++)
                {
                    if (ParentUUIDStructure.Substring(i * 2, 2) != "XX" && inputUUID.Substring(i * 2, 2) != ParentUUIDStructure.Substring(i * 2, 2))
                    {
                        isParent = false;
                    }
                }

                ParentUUID = isParent ? inputUUID : ParentUUID;
                return isParent;
            }
            else
            {
                return false;
            }
        }
    }
}
