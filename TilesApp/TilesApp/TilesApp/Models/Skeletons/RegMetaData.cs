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
    public class RegMetaData
    {
        public string Operation { get; set; }
        public string OperationDetails { get; set; }
        [BsonIgnoreIfNull]
        public Dictionary<string, object> AdditionalData { get; set; }

        //Constructor from json string
        public RegMetaData(string jsonConfig = null)
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
        public RegMetaData(Stream streamConfig)
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
                    if (prop.Name != "AdditionalData")
                    {
                        propertyType.GetProperty(prop.Name).SetValue(this, configData[prop.Name]);
                    }
                    else if (prop.Name == "AdditionalData")
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
    }
}
