using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TilesApp.Models.DataModels;
using Xamarin.Forms;
using static System.Environment;

namespace TilesApp.Services
{
    public static class PHPApi
    {

        private static Dictionary<string, Stream> appsConfigs = new Dictionary<string, Stream> { };
        public static List<ConfigFile> userAppsList = new List<ConfigFile> { };
     
        public async static Task<bool> GetConfigFiles(string user_id, string token)
        {
            try
            {
                var ApplicationDataPath = GetFolderPath(SpecialFolder.LocalApplicationData);
                string[] filesNames = Directory.GetFiles(ApplicationDataPath);

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
                            App.Database.DeleteAllUserApps(App.User.Id);
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
                                        File.Delete(filePath);
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
                        appsConfigs.Add("App_" + cf.AppType + "_" + cf.FileName, stream);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                //MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", e.ToString());
                return false;
            }
        }

        public async static Task<string> GetAppVersion(string token)
        {
            string result = "";
            if (App.IsConnected)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync("https://sherpa.saco.tech/api/getLatestVersion");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            return result;
        }

        public async static Task<string> GetProductTypesList()
        {
            string result = "";
            try
            {
                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.User.OBOToken);
                    HttpResponseMessage response = await client.GetAsync("https://blackboxes.azurewebsites.net/oboria_five/_second-phase/_fields/__index");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return result;
        }
        public static Stream GetAppConfig(string appName)
        {
            try
            {
                return appsConfigs[appName];
            }
            catch
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", "Something went wrong when getting app config file from Web.");
                return null;
            }
        }

    }
}
