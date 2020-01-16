using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace TilesApp.Models.Skeletons
{
    public class JoinMetaData : BaseData
    {
        public String ParentUUID { get; set; }
        [BsonIgnore]
        public String ParentUUIDStructure { get; set; }
        [BsonIgnoreIfNull]
        public Dictionary<string, object> AdditionalData { get; set; }
        [BsonIgnore]
        private List<String> ValidCodeStructure { get; set; }
        [BsonIgnore]
        public bool? IsStationMandatory { get; set; }
        public ObservableCollection<Dictionary<string, object>> ScannerReads { get; set; } = new ObservableCollection<Dictionary<string, object>>();
        //Constructor from json string
        public JoinMetaData(string jsonConfig = null) : base()
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
                            Dictionary<string, object> content = (Dictionary<string, object>)configData[prop.Name];
                            foreach (KeyValuePair<string,object> data in content)
                            {
                                AdditionalData.Add(data.Key, data.Value);
                            }                            
                        }
                        else if (prop.Name == "ValidCodeStructure")
                        {
                            List<string> content = (List<string>)configData[prop.Name];
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
        public JoinMetaData(Stream streamConfig) : base()
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
                    if (configData.ContainsKey(prop.Name))
                    {
                        if (prop.Name == "AdditionalData" & configData[prop.Name] != null)
                        {
                            Dictionary<string, object> content = (Dictionary<string, object>)configData[prop.Name];
                            foreach (KeyValuePair<string, object> data in content)
                            {
                                AdditionalData.Add(data.Key, data.Value);
                            }
                        }
                        else if (prop.Name == "ValidCodeStructure" & configData[prop.Name] != null)
                        {
                            List<string> content = (List<string>)configData[prop.Name];
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
        //Add metadata from string
        public List<string> AddQRMetaData(string jsonConfig)
        {
            List<string> overwrittenFields = new List<string> { };

            try
            {
                Dictionary<string, object> configData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonConfig);
                Type propertyType = GetType();

                foreach (var prop in GetType().GetProperties())
                {
                    if (configData.ContainsKey(prop.Name))
                    {
                        if (prop.Name == "AdditionalData" & configData[prop.Name] != null)
                        {
                            Dictionary<string, object> content = (Dictionary<string, object>)configData[prop.Name];
                            foreach (KeyValuePair<string, object> data in content)
                            {
                                if (!AdditionalData.ContainsKey(data.Key))
                                {
                                    AdditionalData.Add(data.Key, data.Value);
                                }
                                else
                                {
                                    AdditionalData[data.Key] = data.Value;
                                    overwrittenFields.Add(prop.Name + "/" + data.Key);
                                }
                            }
                        }
                        else if (prop.Name == "ValidCodeStructure")
                        {
                            List<string> content = (List<string>)configData[prop.Name];
                            foreach (string data in content)
                            {
                                if (!ValidCodeStructure.Contains(data))
                                {
                                    ValidCodeStructure.Add(data);
                                }
                            }
                        }
                        else
                        {
                            if (propertyType.GetProperty(prop.Name).GetValue(this) != null) { overwrittenFields.Add(prop.Name); }

                            propertyType.GetProperty(prop.Name).SetValue(this, configData[prop.Name]);                       
                        }
                    }
                }
            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "Data is not a compatible JSON (Join).");
            }
            return overwrittenFields;
        }
        public Boolean IsValid()
        {
            var isValid = true;

            foreach (var prop in GetType().GetProperties())
            {
                if (prop.GetValue(this, null) == null & prop.Name != "AdditionalData")
                {
                    if (!(prop.Name == "Station" & IsStationMandatory == false))
                    {                        
                        isValid = false;
                    }                   
                }
            }
            return isValid;
        }
        public void IsParent(String inputUUID)
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
            }
        }
        public Boolean IsValidCode(String inputUUID)
        {
            foreach(string filterUUID in ValidCodeStructure)
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
        public List<string> ProcessInput(Dictionary<string, object> input)
        {
            //First see if it is a JSON file
            try
            {
                List<string> returnList = AddQRMetaData(input[nameof(InputDataProps.Value)].ToString());
                return returnList;
            }
            catch
            {
                //First check if it follows config file code connvention (Aka ValidCodeStructure)
                if (IsValidCode(input[nameof(InputDataProps.Value)].ToString()))
                {
                    //Now check if it is parent
                    IsParent(input[nameof(InputDataProps.Value)].ToString());

                    //Now see if already on list
                    foreach (var item in ScannerReads.ToList())
                    {
                        if (item[nameof(InputDataProps.Value)].ToString() == input[nameof(InputDataProps.Value)].ToString())
                        {
                            return null;
                        }
                    }
                    ScannerReads.Add(input);
                    return null;
                }
                else
                {
                    return null;
                }
            }            
        }
    }
}
