using Newtonsoft.Json;
using PCLAppConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

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

        //CURRENT USER DATA
        public static int? userID;
        public static string userName;
        //Get user validated apps Step 3
        public static List<string> userAppsList = new List<string> { };

        //On start Step 1
        public static void Start(bool forceCacheUpdate = false)
        {
            if (adminID == null)
            {
                Login();
            }
            if (adminID != null)
            {
                GetUsers(forceCacheUpdate);
                GetValidApps(forceCacheUpdate);
            }
        }
        private static void Login()
        {
            
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
                if (!forceCacheUpdate & appsConfigs.ContainsKey(appName))
                {
                    return appsConfigs[appName];
                }
                else
                {
                    if (validAppsList.Count <= 10)
                    {
                        GetConfigFiles(validAppsList);
                    }
                    else
                    {
                        GetConfigFiles(new List<string> { appName });
                    }
                }
                if (appsConfigs.ContainsKey(appName))
                {
                    return appsConfigs[appName];
                }
                else
                {
                    MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "App config file not found in Web. App doesn't seem to exist.");
                    return null;
                }
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
                    if (validAppsList.Contains(userApp)) { userAppsList.Add(userApp); }
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

                appsConfigs.Clear();
                //Dictionary<string, Stream> appsConfigs => key: nameOfApp - value: stream

            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "Something went wrong when getting app(s) config file(s) from Odoo.");
            }
        }

    }
}
