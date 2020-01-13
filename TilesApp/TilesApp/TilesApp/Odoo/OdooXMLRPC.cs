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
        public static int adminID;
        public static List<string> validAppsList = new List<string> { };
        //GET USER APPS
        public static List<string> currentUserAppsList = new List<string> { };
        public static Dictionary<string, Stream> appsConfigs = new Dictionary<string, Stream> { };
        public static void Start()
        {
            Login();
            GetApps();
            //Once user opens app
            //GetConfigFiles(validAppsList);
        }

        private static void GetApps()
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

            //attachment_name
            //datas

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
            Console.WriteLine("The end");
        }

        private static void GetConfigFiles(List<String> appsList)
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

            //attachment_name
            //datas

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
            Console.WriteLine("The end");
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
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
