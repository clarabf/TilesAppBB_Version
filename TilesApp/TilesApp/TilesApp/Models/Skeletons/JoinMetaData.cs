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
    public class JoinMetaData : BaseMetaData
    {
        //Variables to retreive config file parameters -> PROBABLY MOVE TO BASE MODEL
        private Dictionary<string,Dictionary<string, dynamic>> appData = new Dictionary<string,Dictionary<string, dynamic>>{};
        private Dictionary<string,string> appDataIndex  = new Dictionary<string,string>{};
        private Dictionary<string,Dictionary<string, dynamic>> customData = new Dictionary<string,Dictionary<string, dynamic>>{};
        private Dictionary<string,string> customDataIndex  = new Dictionary<string,string>{};
        
        //Fields properties. Maybe move this one to base clase
        [BsonIgnoreIfNull]
        public string ValidCodeFormat
        {
            get
            {
                if(appData[appDataIndex["ValidCodeFormat"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["ValidCodeFormat"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        
        //Fields properties
        [BsonIgnoreIfNull]
        public string ParentUUID
        {
            get
            {
                if(appData[appDataIndex["ParentUUID"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["ParentUUID"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        
        [BsonIgnoreIfNull]
        public string ParentCodeFormat
        {
            get
            {
                if(appData[appDataIndex["ParentCodeFormat"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["ParentCodeFormat"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }   
  
        [BsonIgnoreIfNull]
        public string IsStationRequired
        {
            get
            {
                if(appData[appDataIndex["IsStationRequired"]]["FieldIsSaved"])
                {
                    return appData[appDataIndex["IsStationRequired"]]["DefaultValue(admin)"];
                }
                else
                {
                    return null;
                }
            }
        }
        
        [BsonIgnoreIfNull]
        public Dictionary<string,object> CustomFields
        {
            get
            {  
                Dictionary<string,object> returnDictionary = new Dictionary<string,object>{};
                
                foreach(Dictionary<string, dynamic> field in customData.Values)
                {
                    returnDictionary.Add(field["FieldName"], field["DefaultValue(admin)"]);
                }
                return returnDictionary;
            }
        }        
             
        //Validation before saving
        public Boolean IsValid()
        {
            bool isValid = true;
            
            foreach(Dictionary<string, dynamic> field in appData.Values)
            {
                if(field["IsRequired"] & field["DefaultValue(admin)"] == null){isValid = false;}
            }
            foreach(Dictionary<string, dynamic> field in customData.Values)
            {
                if(field["DefaultValue(admin)"] == null){isValid = false;}
            }

            return isValid;
        }
                
        //Constructor from json stream -> PROBABLY MOVE TO BASE MODEL
        public JoinMetaData(Stream streamConfig) : base(streamConfig)
        {
            try
            {
                if (streamConfig != null)
                {                
                    StreamReader reader = new StreamReader(streamConfig);
                    string jsonConfig = reader.ReadToEnd();
                    streamConfig.Position = 0;

                    Dictionary<string,Dictionary<string,Dictionary<string, dynamic>>> data = JsonConvert.DeserializeObject<Dictionary<string,Dictionary<string,Dictionary<string, dynamic>>>>(jsonConfig);

                    //If no need to go through fields for equality
                    appData = data["AppFields"];
                    customData =data["CustomFields"];

                    //If need to go through fields for equality
                    foreach (KeyValuePair<string,Dictionary<string,dynamic>> field in data["AppFields"])
                    {
                        appDataIndex.Add(field.Value["FieldName"], field.Key);
                        //Continue scripting to fill appData and customData if required
                    }
                    foreach (KeyValuePair<string,Dictionary<string,dynamic>> field in data["CustomFields"])
                    {
                        customDataIndex.Add(field.Value["FieldName"], field.Key);
                        //Continue scripting if required
                    }
                }
            }
            catch
            {
                throw new Exception("Config file is not valid. Maybe there are syntax issues or one or several field names are duplicated.");            
            }
        }
        
        //Constructor from json stream -> PROBABLY MOVE TO BASE MODEL
        public void AddQRMetaData(Dictionary<string, dynamic> data)
        {
            try
            {
                if (data != null)
                {
                    //Go through fields for equality
                    foreach (KeyValuePair<string,dynamic> field in data)
                    {
                        try
                        {   
                            if(appData.ContainsKey(field.Key))
                            {
                                if(field.Value.GetType().ToString().ToLower().Contains(appData[field.Key]["Type"].ToLower()))
                                {
                                    //Add a way to ask for confirmation when QR is going to overwrite one or several fields
                                    appData[field.Key]["DefaultValue(admin)"] = field.Value;          
                                }
                                else
                                {
                                    throw new Exception("One or several of the fields that you are trying to write have been determined as non QR fillable in the config file. Please review QR and/or config file content");
                                }
                            }
                            else if(customData.ContainsKey(field.Key))
                            {
                                if(field.Value.GetType().ToString().ToLower().Contains(customData[field.Key]["Type"].ToLower()))
                                {
                                    //Add a way to ask for confirmation when QR is going to overwrite one or several fields
                                    customData[field.Key]["DefaultValue(admin)"] = field.Value;
                                }
                                else
                                {
                                    throw new Exception("One or several of the fields that you are trying to write have been determined as non QR fillable in the config file. Please review QR and/or config file content");
                                }
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
        
        //
        public override Dictionary<string, object> ProcessedScannerRead(Dictionary<string, object> scannerRead)
        {
            Dictionary<string, object> returnScannerRead = scannerRead;
            //See if it is a QR.
            try
            {
                Dictionary<string, dynamic> data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(scannerRead["Value"].ToString());
                
                if(data != null){AddQRMetaData(data);}
                return null;
            }
            //Try to process as standard read
            catch(Exception e)
            {   
                string duplicatedParentEx = "Last scan had the parent code pattern, but it could not be considered a parent as there was already one assigned. Parent status will be given to the first parent scanned";
                try
                {
                    bool isValidCode = true;
                    
                    foreach(string validContentFormat in appData[appDataIndex["ValidCodeFormat"]]["DefaultValue(admin)"])
                    {
                        isValidCode = true;
                        //Apply filter
                        if (System.Text.RegularExpressions.Regex.IsMatch(validContentFormat, @"\A\b[0-9a-fA-F]+\b\Z") & validContentFormat.Length == 24 & System.Text.RegularExpressions.Regex.IsMatch(scannerRead["Value"].ToString(), @"\A\b[0-9a-fA-FX]+\b\Z") & scannerRead["Value"].ToString().Length == 24)
                        {
                            for (int i = 0; i < 12; i++)
                            {
                                if (validContentFormat.Substring(i * 2, 2) != "XX" && scannerRead["Value"].ToString().Substring(i * 2, 2) != validContentFormat.Substring(i * 2, 2))
                                {
                                    isValidCode = false;
                                }
                            }
                            if(isValidCode){break;}
                        }
                        else
                        {
                            isValidCode = false;
                        }
                    }
                    
                    //<-------This is the method override difference------>
                    if(isValidCode & isParent(scannerRead))
                    {     
                        if(appData[appDataIndex["ParentUUID"]]["DefaultValue(admin)"] != null)
                        {
                            //REPLACE WITH NOTIFICATION OF PARENT ALREADY ASSIGNED
                            throw new Exception(duplicatedParentEx);
                        }
                        else
                        {
                            appData[appDataIndex["ParentUUID"]]["DefaultValue(admin)"] = scannerRead["Value"];
                            returnScannerRead.Add("IsParent",true);
                            return returnScannerRead;
                        }  
                    }
                    //<-------This is the method override difference------>
                    else if(isValidCode)
                    {
                        return returnScannerRead;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch(Exception ex)
                {   
                    //If ex content)
                    if (ex.Message == duplicatedParentEx)
                    {
                        throw ex;
                    }
                    else
                    {
                        throw e;
                    }
                }                          
            }
        }
        
        //Move this to base class. Change name of scanner
        public virtual Dictionary<string, object> ProcessedScannerRead(Dictionary<string,object> scannerRead)
        {
            //See if it is a QR.
            try
            {
                Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(scannerRead["Value"].ToString());
                
                if(data != null){AddQRMetaData(data);}
                return null;
            }
            //Try to process as standard read
            catch(Exception e)
            {
                try
                {
                    bool isValidCode = true;

                    foreach (string validContentFormat in appData[appDataIndex["ValidCodeFormat"]]["DefaultValue(admin)"])
                    {
                        isValidCode = true;

                        //Apply filter
                        if (System.Text.RegularExpressions.Regex.IsMatch(validContentFormat, @"\A\b[0-9a-fA-F]+\b\Z") & validContentFormat.Length == 24 & System.Text.RegularExpressions.Regex.IsMatch(scannerRead["Value"].ToString(), @"\A\b[0-9a-fA-FX]+\b\Z") & scannerRead["Value"].ToString().Length == 24)
                        {
                            for (int i = 0; i < 12; i++)
                            {
                                if (validContentFormat.Substring(i * 2, 2) != "XX" && scannerRead["Value"].ToString().Substring(i * 2, 2) != validContentFormat.Substring(i * 2, 2))
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
    }
}
