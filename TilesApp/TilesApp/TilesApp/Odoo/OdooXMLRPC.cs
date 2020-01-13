using Newtonsoft.Json;
using PCLAppConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlRpc;

namespace TilesApp.Odoo
{
    public static class OdooXMLRPC
    {
        //CONNECTION VARIABLES
        public static string url = ConfigurationManager.AppSettings["SACODOO_URL"];
        public static string db = ConfigurationManager.AppSettings["SACODOO_DB"];
        public static string pass = ConfigurationManager.AppSettings["SACODOO_ADMIN_PASSWORD"];
        public static string user = ConfigurationManager.AppSettings["SACODOO_ADMIN_USER"];
        public static string sherpaFolder = ConfigurationManager.AppSettings["SHERPA_APPS_FOLDER"];

        //CONNECTION CLIENT
        private static XmlRpcClient client = new XmlRpcClient();

        //ODOO RETRIEVED VARIABLES
        public static int? adminID;
        public static Dictionary<string, object> users = new Dictionary<string, object> { };
        public static Dictionary<string, Stream> appsConfigs = new Dictionary<string, Stream> { };
        public static List<string> validAppsList = new List<string> { };

        //CURRENT USER DATA
        public static int? userID;
        public static string userName;
        public static List<string> userAppsList = new List<string> { };

        public static void Start(bool forceCacheUpdate = false)
        {
            if (adminID == null) { Login(); }
            GetUsers(forceCacheUpdate);
            GetValidApps(forceCacheUpdate);
        }
        private static void Login()
        {
            try
            {
                client.Url = url;
                client.Path = "/xmlrpc/2/common";

                // LOGIN
                XmlRpcRequest requestLogin = new XmlRpcRequest("authenticate");
                requestLogin.AddParams(db, user, pass, XmlRpcParameter.EmptyStruct());

                XmlRpcResponse responseLogin = client.Execute(requestLogin);

                if (responseLogin.IsFault())
                {
                    throw new Exception(responseLogin.GetFaultString());
                }
                else
                {
                    adminID = responseLogin.GetInt();
                }
            }
            catch
            {
                throw new Exception("Something went wrong logging to Odoo.");
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

                    Dictionary<string, string> tags = new Dictionary<string, string>();

                    Dictionary<string, object> userInfo;
                    List<object> ids;
                    List<string> id_names;

                    client.Path = "/xmlrpc/2/common";

                    try
                    {
                        // LOGIN IF NECESSARY
                        if (adminID == null) { Login(); };

                        // SEARCH
                        client.Path = "/xmlrpc/2/object";

                        ///////////// TAGS /////////////
                        XmlRpcRequest requestSearch = new XmlRpcRequest("execute_kw");
                        requestSearch.AddParams(db, adminID, pass, "hr.employee.category", "search_read",
                            XmlRpcParameter.AsArray(
                                XmlRpcParameter.AsArray(
                                    XmlRpcParameter.AsArray("id", ">", 0)
                                )
                            ),
                            XmlRpcParameter.AsStruct(
                                XmlRpcParameter.AsMember("fields", XmlRpcParameter.AsArray("name"))
                            )
                        );

                        XmlRpcResponse responseSearch = client.Execute(requestSearch);

                        List<object> responseList = (List<object>)responseSearch.GetObject();
                        foreach (object fields in responseList)
                        {
                            Dictionary<string, object> dict = (Dictionary<string, object>)fields;
                            tags.Add(dict["id"].ToString(), dict["name"].ToString());
                        }

                        //USERS
                        requestSearch = new XmlRpcRequest("execute_kw");
                        requestSearch.AddParams(db, adminID, pass, "hr.employee", "search_read",
                            XmlRpcParameter.AsArray(
                                XmlRpcParameter.AsArray(
                                    XmlRpcParameter.AsArray("id", ">", 0)
                                )
                            ),
                            XmlRpcParameter.AsStruct(
                                XmlRpcParameter.AsMember("fields", XmlRpcParameter.AsArray("name", "barcode", "category_ids"))
                            )
                        );

                        responseSearch = client.Execute(requestSearch);

                        Console.WriteLine(responseSearch.GetString());
                        responseList = (List<object>)responseSearch.GetObject();
                        foreach (object fields in responseList)
                        {
                            Dictionary<string, object> dict = (Dictionary<string, object>)fields;
                            userInfo = new Dictionary<string, object>();
                            id_names = new List<string>();
                            if (dict["barcode"].ToString() != "False")
                            {
                                ids = (List<object>)dict["category_ids"];
                                foreach (object id in ids) id_names.Add(tags[id.ToString()]);
                                userInfo.Add("id", dict["id"].ToString());
                                userInfo.Add("name", dict["name"].ToString());
                                userInfo.Add("tags", id_names);
                                users.Add(dict["barcode"].ToString(), userInfo);
                            }
                        }
                    }
                    catch (WebServiceException)
                    {
                        users.Add("error", "internet");
                    }
                    catch (System.InvalidCastException)
                    {
                        users.Add("error", "odoo");
                    }
                }
            }
            catch
            {
                throw new Exception("Something went wrong getting users from Odoo.");
            }
        }
        private static void GetValidApps(bool forceCacheUpdate = false)
        {
            try
            {
                client.Path = "/xmlrpc/2/object";
                XmlRpcRequest requestCreate = new XmlRpcRequest("execute_kw");
                requestCreate.AddParams(db, adminID, pass, "documents.document", "search_read",
                        XmlRpcParameter.AsArray(
                            XmlRpcParameter.AsArray(
                                "&",
                                XmlRpcParameter.AsArray("owner_id", "=", adminID),
                                XmlRpcParameter.AsArray("folder_id", "=", sherpaFolder)
                            )
                        ),
                        XmlRpcParameter.AsStruct(
                            XmlRpcParameter.AsMember("fields", XmlRpcParameter.AsArray("attachment_name"))
                        )
                    );

                XmlRpcResponse responseCreate = client.Execute(requestCreate);

                if (responseCreate.IsFault())
                {
                    Console.WriteLine(responseCreate.GetFaultString());
                }
                else
                {
                    List<object> responseList = (List<object>)responseCreate.GetObject();
                    foreach (object fields in responseList)
                    {
                        Dictionary<string, object> dict = (Dictionary<string, object>)fields;
                        if (dict.ContainsKey("attachment_name"))
                        {
                            if (dict["attachment_name"].ToString().StartsWith("App_"))
                            {
                                validAppsList.Add(dict["attachment_name"].ToString().Substring(0, dict["attachment_name"].ToString().Length - 5));
                            };
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("Something went wrong getting valid apps from Odoo.");
            }
        }
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
                    throw new Exception("App config file not found in Odoo. App doesn't seem to exist.");
                }
            }
            catch
            {
                throw new Exception("Something went wrong getting app config file from Odoo.");
            }
        }
        public static void SetCurrentUser(string barcode, bool forceCacheUpdate = false)
        {
            if (forceCacheUpdate) { userAppsList = null; }
            try
            {
                Dictionary<string, object> selectedUser = (Dictionary<string, object>)users[barcode];
                userName = (string)selectedUser["name"];
                userID = (int)selectedUser["id"];
                foreach (string userApp in (List<string>)selectedUser["tags"])
                {
                    if (validAppsList.Contains(userApp)) { userAppsList.Add(userApp) };
                }
            }
            catch
            {
                throw new Exception("Something went wrong setting current user.");
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

                client.Path = "/xmlrpc/2/object";
                XmlRpcRequest requestCreate = new XmlRpcRequest("execute_kw");
                requestCreate.AddParams(db, adminID, pass, "documents.document", "search_read",
                        XmlRpcParameter.AsArray(
                            XmlRpcParameter.AsArray(
                                "&",
                                XmlRpcParameter.AsArray("owner_id", "=", adminID),
                                "&",
                                XmlRpcParameter.AsArray("folder_id", "=", sherpaFolder),
                                XmlRpcParameter.AsArray("attachment_name", "in", requestList)
                            )
                        ),
                        XmlRpcParameter.AsStruct(
                            XmlRpcParameter.AsMember("fields", XmlRpcParameter.AsArray("attachment_name", "datas"))
                        )
                    );

                XmlRpcResponse responseCreate = client.Execute(requestCreate);

                if (responseCreate.IsFault())
                {
                    Console.WriteLine(responseCreate.GetFaultString());
                }
                else
                {
                    List<object> responseList = (List<object>)responseCreate.GetObject();
                    foreach (object fields in responseList)
                    {
                        Dictionary<string, object> dict = (Dictionary<string, object>)fields;
                        string nameOfApp = dict["attachment_name"].ToString().Substring(0, dict["attachment_name"].ToString().Length - 5);
                        byte[] byteArray = Convert.FromBase64String(dict["datas"].ToString());
                        MemoryStream stream = new MemoryStream(byteArray);

                        appsConfigs.Add(nameOfApp, stream);
                    }
                }
            }
            catch
            {
                throw new Exception("Something went wrong getting app(s) config file(s) from Odoo.");
            }
        }
    }
}
