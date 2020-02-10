using MongoDB.Bson.Serialization.Attributes;
using System;
using Xamarin.Essentials;
using TilesApp.Services;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

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
        protected Dictionary<string, Dictionary<string, dynamic>> predefinedData = new Dictionary<string, Dictionary<string, dynamic>> { };

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
            set
            {
                appData[appDataIndex["Station"]]["DefaultValue(admin)"] = value;
            }
        }
        [BsonIgnoreIfNull]
        public Models.Location Location
        {
            get
            {
                try
                {
                    return App.GeoLocation;
                }
                catch
                {
                    return null;
                }               
            }
        }
        [BsonIgnoreIfNull]
        public ObservableCollection<Dictionary<string, object>> ScannerReads { get; set; } = new ObservableCollection<Dictionary<string, object>>();
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
            GetDeviceLocation();
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
                    if (data.ContainsKey("PredefinedValues")) {
                        predefinedData = data["PredefinedValues"];
                    }

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
            catch (Exception e)
            {
                throw new Exception("Config file is not valid. Maybe there are syntax issues or one or several field names are duplicated.");
            }
        }

        //Method to process scanner reads and discriminate between: Ones with scans with not valid UUID, scans that are QR Jsons, those that are overwritting already written fields and valid scans
        public virtual Dictionary<string, object> ProcessScannerRead(Dictionary<string, object> scannerRead)
        {
            Dictionary<string,object> result = new Dictionary<string, object>();
            //See if it is a QR.
            try
            {
                Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(scannerRead["Value"].ToString());
                if (data != null) { result = AddAddiontalMetaData(data, "QR"); }
                return result;
            }
            //Try to process as a barcode (barcode with predefined values or standard read)
            catch (Exception e)
            {
                if (predefinedData.ContainsKey(scannerRead["Value"].ToString()))
                {
                    try
                    {
                        Dictionary<string, object> data = predefinedData[scannerRead["Value"].ToString()];
                        if (data != null) { result = AddAddiontalMetaData(data, "barcode"); }
                        return result;
                    }
                    catch (Exception e2)
                    {
                        result.Add("Error", e2.Message);
                        return result;
                    }
                }
                else
                {
                    try
                    {
                        bool isValidCode = true;

                        foreach (string validContentFormat in appData[appDataIndex["ValidCodeFormat"]]["DefaultValue(admin)"])
                        {
                            isValidCode = true;

                            //Apply filter
                            if (System.Text.RegularExpressions.Regex.IsMatch(validContentFormat, @"\b([\w$_-]+)\b") & System.Text.RegularExpressions.Regex.IsMatch(scannerRead["Value"].ToString(), @"\b([\w_-]+)\b") & validContentFormat.Length == scannerRead["Value"].ToString().Length)
                            {
                                for (int i = 0; i < validContentFormat.Length; i++)
                                {
                                    if (validContentFormat.ToUpper()[i] != '$' && scannerRead["Value"].ToString().ToUpper()[i] != validContentFormat.ToUpper()[i])
                                    {
                                        isValidCode = false;
                                        break; // next validContentFormat
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
                            ScannerReads.Add(scannerRead);
                            result = scannerRead;
                        }
                        return result;
                    }
                    catch
                    {
                        return result;
                    }
                }  
            }
        }

        //Add metadata from QR Json method
        public Dictionary<string, object> AddAddiontalMetaData(Dictionary<string, dynamic> data, string type)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
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
                                    result.Add( "Error", "One or several of the fields that you are trying to write have been determined as non fillable in the config file. Please review " + type + " and/or config file content");
                                    return result;
                                }
                            }
                            else if (customData.ContainsKey(field.Key))
                            {
                                //Add a way to ask for confirmation when QR is going to overwrite one or several fields
                                customData[field.Key]["DefaultValue(admin)"] = field.Value;
                            }
                            else
                            {
                                result.Add("Error", "One or several of the fields in the " + type + " do not exist on the config file. Please review " + type + " content.");
                                return result;
                            }
                        }
                        catch (Exception e)
                        {
                            result.Add("Error", e.Message);
                            return result;
                        }
                    }
                }
            }
            catch
            {
                result.Add("Error", type + " content is not valid. Maybe there are syntax issues or the content type of an AppData field does not match the required type.");
                return result;
            }
            return result;
        }

        //Validation before saving
        public List<string> IsValid()
        {
            List<string> result = new List<string>();
            try
            {
                foreach (Dictionary<string, dynamic> field in appData.Values)
                {
                    if (field["IsRequired"] & field["DefaultValue(admin)"] == null)
                    {
                        result.Add(field["FieldName"]);
                    }
                }
                foreach (Dictionary<string, dynamic> field in customData.Values)
                {
                    if (field["IsRequired(admin)"] && field["DefaultValue(admin)"] == null)
                    {
                        result.Add(field["FieldName(admin)"]);
                    }
                }
            }
            catch
            {
                result.Add("Unknown-field-name");
            }
            return result;
        }

        // Refresh Geographical location of the App
        private async void GetDeviceLocation()
        {
            Xamarin.Essentials.Location rowGeoLocation = new Xamarin.Essentials.Location();
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {
                    rowGeoLocation = Geolocation.GetLastKnownLocationAsync().Result;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    Console.WriteLine("Location Denied", "Can not continue, try again.", "OK");
                }

            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch
            {
                return;
            }
            if (rowGeoLocation != null)
            {
                if (App.GeoLocation != null)
                {
                    if (!App.GeoLocation.lat.Equals(rowGeoLocation.Latitude.ToString()) || !App.GeoLocation.lon.Equals(rowGeoLocation.Longitude.ToString()))
                    {
                        App.GeoLocation = await HttpClientManager.ReverseGeoCodeAsync(rowGeoLocation.Latitude.ToString(), rowGeoLocation.Longitude.ToString());
                    }
                }
                else
                {
                    App.GeoLocation = await HttpClientManager.ReverseGeoCodeAsync(rowGeoLocation.Latitude.ToString(), rowGeoLocation.Longitude.ToString());
                }

            }
        }
    }
}
