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
    public class JoinMetaData : BaseMetaData
    {
        private string _parentUUID;
        //Fields properties
        [BsonIgnoreIfNull]
        public string ParentUUID
        {
            get
            {
                try
                {
                    if (appData[appDataIndex["ParentUUID"]]["FieldIsSaved"])
                    {
                        return appData[appDataIndex["ParentUUID"]]["DefaultValue(admin)"];
                    }
                }
                catch { }
                return _parentUUID;
            }
            set
            {
                try
                {
                    appData[appDataIndex["ParentUUID"]]["DefaultValue(admin)"] = value;
                }
                catch {
                    _parentUUID = value;
                }
            }
        }        
        [BsonIgnoreIfNull]
        public string ParentCodeFormat
        {
            get
            {
                try
                {
                    if (appData[appDataIndex["ParentCodeFormat"]]["FieldIsSaved"])
                    {
                        return appData[appDataIndex["ParentCodeFormat"]]["DefaultValue(admin)"];
                    }
                }
                catch { }
                return null;
            }
        }   

        //Constructor from json stream
        public JoinMetaData(Stream streamConfig) : base(streamConfig){}        

        public override Dictionary<string, object> ProcessScannerRead(Dictionary<string, object> scannerRead)
        {
            Dictionary<string, object> returnScannerRead = base.ProcessScannerRead(scannerRead); ;
            if(returnScannerRead.Count>0 & IsParent(scannerRead))
            {     
                if(appData[appDataIndex["ParentUUID"]]["DefaultValue(admin)"] != null)
                {
                    //REPLACE WITH NOTIFICATION OF PARENT ALREADY ASSIGNED                   
                    MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "Last scan had the parent code pattern, but it could not be considered a parent as there was already one assigned. Parent status will be given to the first parent scanned");
                    return new Dictionary<string, object>();
                }
                else
                {
                    appData[appDataIndex["ParentUUID"]]["DefaultValue(admin)"] = scannerRead["Value"];
                    ScannerReads[ScannerReads.IndexOf(scannerRead)].Add("IsParent", true);
                    return returnScannerRead;
                }
            }
            else
            {
                return returnScannerRead;
            }
        }        

        private bool IsParent(Dictionary<string,object> scannerRead)
        {
            bool isParent = true;
            //Apply filter
            if (System.Text.RegularExpressions.Regex.IsMatch(appData[appDataIndex["ParentCodeFormat"]]["DefaultValue(admin)"], @"\b([a-fA-F0-9xX]+)\b") & System.Text.RegularExpressions.Regex.IsMatch(scannerRead["Value"].ToString(), @"\b([a-fA-F0-9]+)\b") & appData[appDataIndex["ParentCodeFormat"]]["DefaultValue(admin)"].Length == scannerRead["Value"].ToString().Length )
            {
                for (int i = 0; i < appData[appDataIndex["ParentCodeFormat"]]["DefaultValue(admin)"].Length; i++)
                {
                    if (appData[appDataIndex["ParentCodeFormat"]]["DefaultValue(admin)"].ToUpper()[i] != 'X' && scannerRead["Value"].ToString().ToUpper()[i] != appData[appDataIndex["ParentCodeFormat"]]["DefaultValue(admin)"].ToUpper()[i])
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
