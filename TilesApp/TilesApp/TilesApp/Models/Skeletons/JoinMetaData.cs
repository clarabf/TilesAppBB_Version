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

        //Constructor from json stream
        public JoinMetaData(Stream streamConfig) : base(streamConfig){}        

        public override Dictionary<string, object> ProcessScannerRead(Dictionary<string, object> scannerRead)
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
                        if (System.Text.RegularExpressions.Regex.IsMatch(validContentFormat, @"\b([a-fA-F0-9xX]+)\b") & validContentFormat.Length == 24 & System.Text.RegularExpressions.Regex.IsMatch(scannerRead["Value"].ToString(), @"\b([a-fA-F0-9]+)\b") & scannerRead["Value"].ToString().Length == 24)
                        {
                            for (int i = 0; i < 12; i++)
                            {
                                if (validContentFormat.Substring(i * 2, 2).ToUpper() != "XX" && scannerRead["Value"].ToString().Substring(i * 2, 2).ToUpper() != validContentFormat.Substring(i * 2, 2).ToUpper())
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
                    if(isValidCode & IsParent(scannerRead))
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
                            ScannerReads.Add(returnScannerRead);
                            return returnScannerRead;
                        }  
                    }
                    //<-------This is the method override difference------>
                    else if(isValidCode)
                    {
                        ScannerReads.Add(returnScannerRead);
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

        private bool IsParent(Dictionary<string,object> scannerRead)
        {
            bool isParent = true;
            //Apply filter
            if (System.Text.RegularExpressions.Regex.IsMatch(appData[appDataIndex["ParentCodeFormat"]]["DefaultValue(admin)"], @"\b([a-fA-F0-9xX]+)\b") & appData[appDataIndex["ParentCodeFormat"]]["DefaultValue(admin)"].Length == 24 & System.Text.RegularExpressions.Regex.IsMatch(scannerRead["Value"].ToString(), @"\b([a-fA-F0-9]+)\b") & scannerRead["Value"].ToString().Length == 24)
            {
                for (int i = 0; i < 12; i++)
                {
                    if (appData[appDataIndex["ParentCodeFormat"]]["DefaultValue(admin)"].Substring(i * 2, 2).ToUpper() != "XX" && scannerRead["Value"].ToString().Substring(i * 2, 2).ToUpper() != appData[appDataIndex["ParentCodeFormat"]]["DefaultValue(admin)"].Substring(i * 2, 2).ToUpper())
                    {
                        isParent = false;
                    }
                }
            }
            else
            {
                isParent = false;
            }

            return isParent;
        }        
    }
}
