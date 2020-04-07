using Newtonsoft.Json;
using PCLAppConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesApp.Models.DataModels;
using Xamarin.Forms;
using static System.Environment;

namespace TilesApp.Services
{
    public static class PHPApi
    {
        //CONNECTION VARIABLES
        public static string url = ConfigurationManager.AppSettings["SACODOO_URL"];
        public static string db = ConfigurationManager.AppSettings["SACODOO_DB"];
        public static string pass = ConfigurationManager.AppSettings["SACODOO_ADMIN_PASSWORD"];
        public static string user = ConfigurationManager.AppSettings["SACODOO_ADMIN_USER"];
        public static string sherpaFolder = ConfigurationManager.AppSettings["SHERPA_APPS_FOLDER"];

        //ODOO RETRIEVED VARIABLES
        public static int? adminID;
        public static Dictionary<string, object> users = new Dictionary<string, object> { };
        public static List<string> validAppsList = new List<string> { };
        public static Dictionary<string, Stream> appsConfigs = new Dictionary<string, Stream> { };
        public static List<ConfigFile> dbConfigs = new List<ConfigFile>();

        //CURRENT USER DATA
        public static int? userID;
        public static string userName;
        //Get user validated apps Step 3
        public static List<ConfigFile> userAppsList = new List<ConfigFile> { };

        //On start Step 1
        public static void Start(bool forceCacheUpdate = false)
        {
            Download();
            GetUsers(forceCacheUpdate);
            GetValidApps(forceCacheUpdate);
        }
        private static void Download()
        {

            var JSONParser = new JSONParser();
            var ApplicationDataPath = GetFolderPath(SpecialFolder.LocalApplicationData);

            // Here we will download the config files from the web
            File.WriteAllText(Path.Combine(ApplicationDataPath, "App_QC_testQC.json"), JSONParser.QC_JSON());
            File.WriteAllText(Path.Combine(ApplicationDataPath, "App_Link_testLink.json"), JSONParser.Link_JSON());
            File.WriteAllText(Path.Combine(ApplicationDataPath, "App_Join_testJoin.json"), "Join");
            File.WriteAllText(Path.Combine(ApplicationDataPath, "App_Reg_testReg.json"), "Reg");
            File.WriteAllText(Path.Combine(ApplicationDataPath, "App_Review_testReview.json"), "Review");

            //File.Delete(Path.Combine(ApplicationDataPath, "test.json"));
            //File.Delete(Path.Combine(ApplicationDataPath, "App_QC_Test.json"));
            //File.Delete(Path.Combine(ApplicationDataPath, "App_Link_Test.json"));
            //File.Delete(Path.Combine(ApplicationDataPath, "App_Join_Test.json"));
            //File.Delete(Path.Combine(ApplicationDataPath, "App_Reg_Test.json"));
            //File.Delete(Path.Combine(ApplicationDataPath, "App_Review_Test.json"));

            string[] filesNames = Directory.GetFiles(ApplicationDataPath);
            appsConfigs.Clear();

            for (int i = 0; i < filesNames.Length; i++)
            {

                string[] appNameArr = filesNames[i].Split('/');

                string fileName = appNameArr[appNameArr.Length - 1];
                string filePath = filesNames[i];

                if (fileName.Contains(".json"))
                {
                    fileName = fileName.Substring(0, fileName.Length - 5);
                    string[] typeAndName = fileName.Split('_');

                    if (typeAndName.Length == 3)
                    {
                        FileStream fs = File.OpenRead(filesNames[i]);
                        MemoryStream stream = new MemoryStream();
                        fs.CopyTo(stream);

                        appsConfigs.Add(fileName, stream);

                        ConfigFile cf = new ConfigFile()
                        {
                            FileName = typeAndName[2],
                            FilePath = filePath,
                            AppType = typeAndName[1],
                        };

                        dbConfigs.Add(cf);
                    }
                }
            }
        }
        public static void GetUsers(bool forceCacheUpdate = false)
        {
            try
            {
                // CHECK IF ALREADY CACHED
                if (users == null || !forceCacheUpdate)
                {
                    // OTHERWISE DO
                    List<string> names = new List<string>();

                    List<string> tags = new List<string>();
                    Dictionary<string, object> userInfo;
                  
                    users.Clear();
                    // Dictionary<string, object> users => key: barcode - value: userInfo
                    // Dictionary<string, object> userInfo; 
                    // - id (int)
                    // - name (string)
                    // - tags (List<string>)

                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
        private static void GetValidApps(bool forceCacheUpdate = false)
        {
            try
            {
                validAppsList.Clear();
                // List<string> validAppsList => name of the apps (jsons)
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
        //Get app config Step 4
        public static Stream GetAppConfig(string appName, bool forceCacheUpdate = false)
        {
            try
            {
                //Ask for file in DB. If conexion error, return stream from db
                return appsConfigs[appName];

                //Search file in DB

                //if (!forceCacheUpdate & appsConfigs.ContainsKey(appName))
                //{
                //    return appsConfigs[appName];
                //}
                //else
                //{
                //    if (validAppsList.Count <= 10)
                //    {
                //        GetConfigFiles(validAppsList);
                //    }
                //    else
                //    {
                //        GetConfigFiles(new List<string> { appName });
                //    }
                //}
                //if (appsConfigs.ContainsKey(appName))
                //{
                //    return appsConfigs[appName];
                //}
                //else
                //{
                //    MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "App config file not found in Web. App doesn't seem to exist.");
                //    return null;
                //}
            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "Something went wrong when getting app config file from Web.");
                return null;
            }
        }
        //Once user scans barcode Step 2
        public static void SetCurrentUser(string barcode)
        {
            userAppsList.Clear();
            try
            {
                Dictionary<string, object> selectedUser = (Dictionary<string, object>)users[barcode];
                userName = (string)selectedUser["name"];
                userID = Int32.Parse(selectedUser["id"].ToString());
                foreach (string userApp in (List<string>)selectedUser["tags"])
                {
                    //check if is valid
                    //if (validAppsList.Contains(userApp)) { userAppsList.Add(userApp); }
                }
            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "Something went wrong when setting current user.");
            }
        }
        private static void GetConfigFiles(List<String> appsList, bool forceCacheUpdate = false)
        {
            try
            {
                List<object> requestList = new List<object> { };

                foreach (var requestApp in appsList)
                {
                    requestList.Add(requestApp + ".json");
                }

                //appsConfigs.Clear();
                //Dictionary<string, Stream> appsConfigs => key: nameOfApp - value: stream

            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "Something went wrong when getting app(s) config file(s) from Odoo.");
            }
        }

    }
}
