using MongoDB.Bson.Serialization.Attributes;
using System;
using Xamarin.Essentials;
using TilesApp.Services;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TilesApp.Models
{
    public class BaseMetaData
    {
        //Variables for global usage
        public enum InputDataProps
        {
            Value,
            Timestamp,
            ReaderType,
            ReaderSerialNumber,
        }

        //Variables to retreive config file parameters
        protected Dictionary<string, Dictionary<string, dynamic>> appData = new Dictionary<string, Dictionary<string, dynamic>> { };
        protected Dictionary<string, string> appDataIndex = new Dictionary<string, string> { };
        protected Dictionary<string, Dictionary<string, dynamic>> customData = new Dictionary<string, Dictionary<string, dynamic>> { };
        protected Dictionary<string, string> customDataIndex = new Dictionary<string, string> { };

        //Fields properties
        [BsonIgnoreIfNull]
        public string AppName { get; set; }
        [BsonIgnoreIfNull]
        public string AppType { get; set; }
        [BsonIgnoreIfNull]
        public int? UserId
        { 
            get
            {
                return OdooXMLRPC.userID;
            }
        }
        [BsonIgnoreIfNull]
        public string UserName
        {
            get
            {
                return OdooXMLRPC.userName;
            }
        }
        [BsonIgnoreIfNull]
        public string DeviceSerialNumber
        {
            get
            {
                return App.DeviceSerialNumber != null ? App.DeviceSerialNumber : null;
            }
        }
        [BsonIgnoreIfNull]
        public string Station
        {
            get
            {
                if (appData[appDataIndex["Station"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["Station"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        [BsonIgnoreIfNull]
        public Location Location
        {
            get
            {
                try
                {
                    return Geolocation.GetLastKnownLocationAsync().Result;
                }
                catch
                {
                    return null;
                }
            }
        }
        [BsonIgnoreIfNull]
        public Dictionary<string, object> CustomFields
        {
            get
            {
                Dictionary<string, object> returnDictionary = new Dictionary<string, object> { };

                foreach (Dictionary<string, dynamic> field in customData.Values)
                {
                    returnDictionary.Add(field["FieldName(admin)"], field["DefaultValue(admin)"]);
                }
                return returnDictionary;
            }
        }

        //Constructor from json stream
        public BaseMetaData(Stream streamConfig)
        {
            try
            {
                if (streamConfig != null)
                {
                    StreamReader reader = new StreamReader(streamConfig);
                    string jsonConfig = reader.ReadToEnd();
                    streamConfig.Position = 0;

                    Dictionary<string, Dictionary<string, Dictionary<string, dynamic>>> data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, dynamic>>>>(jsonConfig);

                    //If no need to go through fields for equality
                    appData = data["AppFields"];
                    customData = data["CustomFields"];

                    //If need to go through fields for equality
                    foreach (KeyValuePair<string, Dictionary<string, dynamic>> field in data["AppFields"])
                    {
                        appDataIndex.Add(field.Value["FieldName"], field.Key);
                        //Continue scripting to fill appData and customData if required
                    }
                    foreach (KeyValuePair<string, Dictionary<string, dynamic>> field in data["CustomFields"])
                    {
                        customDataIndex.Add(field.Value["FieldName(admin)"], field.Key);
                        //Continue scripting if required
                    }
                }
            }
            catch
            {
                throw new Exception("Config file is not valid. Maybe there are syntax issues or one or several field names are duplicated.");
            }
        }

        //Method to process scanner reads and discriminate between: Ones with scans with not valid UUID, scans that are QR Jsons, those that are overwritting already written fields and valid scans
        public virtual Dictionary<string, object> ProcessScannerRead(Dictionary<string, object> scannerRead)
        {
            //See if it is a QR.
            try
            {
                Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(scannerRead["Value"].ToString());

                if (data != null) { AddQRMetaData(data); }
                return null;
            }
            //Try to process as standard read
            catch (Exception e)
            {
                try
                {
                    bool isValidCode = true;

                    foreach (string validContentFormat in appData[appDataIndex["ValidCodeFormat"]]["DefaultValue(admin)"])
                    {
                        isValidCode = true;

                        //Apply filter
                        if (System.Text.RegularExpressions.Regex.IsMatch(validContentFormat, @"\b([a-fA-F0-9xX]+)\b") & validContentFormat.Length == 24 & System.Text.RegularExpressions.Regex.IsMatch(scannerRead["Value"].ToString(), @"\b([a-fA-F0-9]+)\b") & scannerRead["Value"].ToString().Length == 24)
                        {
                            for (int i = 0; i < 12; i++)
                            {
                                if (validContentFormat.Substring(i * 2, 2).ToUpper() != "XX" && scannerRead["Value"].ToString().Substring(i * 2, 2).ToUpper() != validContentFormat.Substring(i * 2, 2).ToUpper())
                                {
                                    isValidCode = false;
                                }
                            }
                            if (isValidCode) { break; }
                        }
                        else
                        {
                            isValidCode = false;
                        }
                    }

                    if (isValidCode)
                    {
                        return scannerRead;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    throw e;
                }
            }
        }

        //Add metadata from QR Json method
        public void AddQRMetaData(Dictionary<string, dynamic> data)
        {
            try
            {
                if (data != null)
                {
                    //Go through fields for equality
                    foreach (KeyValuePair<string, dynamic> field in data)
                    {
                        try
                        {
                            if (appData.ContainsKey(field.Key))
                            {
                                if (field.Value.GetType().ToString().ToLower().Contains(appData[field.Key]["Type"].ToLower()))
                                {
                                    //Add a way to ask for confirmation when QR is going to overwrite one or several fields
                                    appData[field.Key]["DefaultValue(admin)"] = field.Value;
                                }
                                else
                                {
                                    throw new Exception("One or several of the fields that you are trying to write have been determined as non QR fillable in the config file. Please review QR and/or config file content");
                                }
                            }
                            else if (customData.ContainsKey(field.Key))
                            {
                                //Add a way to ask for confirmation when QR is going to overwrite one or several fields
                                customData[field.Key]["DefaultValue(admin)"] = field.Value;
                            }
                            else
                            {
                                throw new Exception("One or several of the fields in the QR do not exist on the config file. Please review QR content.");
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("QR content is not valid. Maybe there are syntax issues or the content type of an AppData field does not match the required type.");
            }
        }

        //Validation before saving
        public Boolean IsValid()
        {
            bool isValid = true;

            foreach (Dictionary<string, dynamic> field in appData.Values)
            {
                if (field["IsRequired"] & field["DefaultValue(admin)"] == null) { isValid = false; }
            }
            foreach (Dictionary<string, dynamic> field in customData.Values)
            {
                if (field["DefaultValue(admin)"] == null) { isValid = false; }
            }

            return isValid;
        }
    }
}
