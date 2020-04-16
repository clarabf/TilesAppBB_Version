using Newtonsoft.Json;
using PCLAppConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TilesApp.Models.DataModels;
using Xamarin.Forms;
using static System.Environment;

namespace TilesApp.Services
{
    public static class PHPApi
    {
        //ODOO RETRIEVED VARIABLES
        public static int? adminID;
        public static Dictionary<string, object> users = new Dictionary<string, object> { };
        public static List<string> validAppsList = new List<string> { };
        public static Dictionary<string, Stream> appsConfigs = new Dictionary<string, Stream> { };

        //CURRENT USER DATA
        public static int? userID;
        public static string userName;
        //Get user validated apps Step 3
        public static List<ConfigFile> userAppsList = new List<ConfigFile> { };

        //On start Step 1
        public static void Start(bool forceCacheUpdate = false)
        {
            //Download();
        }
        public async static void GetUsers(string token)
        {
            try
            {
                // CHECK IF ALREADY CACHED
                if (users == null)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage response = await client.GetAsync("https://sherpa.saco.tech/api/getUsers");
                    string responseString = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        int i = 0;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
        public async static void GetValidApps(string token)
        {
            try
            {
                validAppsList.Clear();
                // List<string> validAppsList => name of the apps (jsons)
                //get data from API
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync("https://sherpa.saco.tech/api/getApps");
                string responseString = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    int i = 0;
                }

            }
            catch (Exception e)
            {
                //throw new Exception(e.ToString());
            }
        }
        
        public static Stream GetAppConfig(string appName, bool forceCacheUpdate = false)
        {
            try
            {
                //Ask for file in DB. If conexion error, return stream from db
                return appsConfigs[appName];
            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "Something went wrong when getting app config file from Web.");
                return null;
            }
        }
        
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
        public async static Task<bool> GetConfigFiles(string user_id, string token)
        {
            try
            {
                var ApplicationDataPath = GetFolderPath(SpecialFolder.LocalApplicationData);

                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id",user_id)
                    });
                    HttpResponseMessage response = await client.PostAsync("https://sherpa.saco.tech/api/getUserApps", content);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        Dictionary<string, object> responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);

                        if (responseDict.ContainsKey("content"))
                        {
                            appsConfigs.Clear();
                            userAppsList.Clear();
                            var lala = responseDict["content"];
                            Dictionary<string, string> userAppsDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(lala.ToString());

                            // We check the user apps => Key: App_name - Value: App_content (json)
                            foreach (KeyValuePair<string, string> kvp in userAppsDict)
                            {
                                string fileName = kvp.Key;

                                if (fileName.Contains(".json"))
                                {
                                    fileName = fileName.Substring(0, fileName.Length - 5);
                                    string[] typeAndName = fileName.Split('_');

                                    if (typeAndName.Length == 3)
                                    {
                                        string filePath = Path.Combine(ApplicationDataPath, kvp.Key);
                                        File.WriteAllText(filePath, kvp.Value);
                                        FileStream fs = File.OpenRead(filePath);
                                        MemoryStream stream = new MemoryStream();
                                        fs.CopyTo(stream);

                                        appsConfigs.Add(fileName, stream);

                                        ConfigFile cf = new ConfigFile()
                                        {
                                            FileName = typeAndName[2],
                                            FilePath = filePath,
                                            AppType = typeAndName[1],
                                        };

                                        int id = App.Database.SaveConfigFile(cf);
                                        UserApp userApp = new UserApp() { UserId = App.User.Id, ConfigFileId = id };
                                        App.Database.SaveUserApp(userApp);
                                        userAppsList.Add(cf);
                                    } // correct format
                                } // file is a json
                            } // key-value pairs
                        } // response has returned success
                    } // response okay
                }
                //Get from DB
                else
                {
                    userAppsList.Clear();
                    appsConfigs.Clear();
                    
                    userAppsList = App.Database.GetUserConfigFiles(App.User.Id);
                    foreach (ConfigFile cf in userAppsList)
                    {
                        FileStream fs = File.OpenRead(cf.FilePath);
                        MemoryStream stream = new MemoryStream();
                        fs.CopyTo(stream);
                        appsConfigs.Add(cf.FileName, stream);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", e.ToString());
                return false;
            }
        }

        private static void RecoverFiles()
        {
            var ApplicationDataPath = GetFolderPath(SpecialFolder.LocalApplicationData);

            //var JSONParser = new JSONParser();
            //File.WriteAllText(Path.Combine(ApplicationDataPath, "App_QC_testQC.json"), JSONParser.QC_JSON());
            //File.WriteAllText(Path.Combine(ApplicationDataPath, "App_Link_testLink.json"), JSONParser.Link_JSON());
            //File.WriteAllText(Path.Combine(ApplicationDataPath, "App_Join_testJoin.json"), JSONParser.Join_JSON());
            //File.WriteAllText(Path.Combine(ApplicationDataPath, "App_Reg_testReg.json"), JSONParser.Reg_JSON());
            //File.WriteAllText(Path.Combine(ApplicationDataPath, "App_Review_testReview.json"), JSONParser.Review_JSON());

            //File.Delete(Path.Combine(ApplicationDataPath, "test.json"));
            //File.Delete(Path.Combine(ApplicationDataPath, "App_QC_Test.json"));
            //File.Delete(Path.Combine(ApplicationDataPath, "App_Link_Test.json"));
            //File.Delete(Path.Combine(ApplicationDataPath, "App_Join_Test.json"));
            //File.Delete(Path.Combine(ApplicationDataPath, "App_Reg_Test.json"));
            //File.Delete(Path.Combine(ApplicationDataPath, "App_Review_Test.json"));

            string[] filesNames = Directory.GetFiles(ApplicationDataPath);
            
            appsConfigs.Clear();

            foreach (ConfigFile cf in userAppsList)
            {

                FileStream fs = File.OpenRead(cf.FilePath);
                MemoryStream stream = new MemoryStream();
                fs.CopyTo(stream);
                appsConfigs.Add(cf.FileName, stream);
            }
        }

    }
}
