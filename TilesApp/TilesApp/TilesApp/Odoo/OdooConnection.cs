////
/// Copyright (c) 2016 Saúl Piña <sauljabin@gmail.com>.
/// 
/// This file is part of xmlrpcwsc.
/// 
/// xmlrpcwsc is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Lesser General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
/// 
/// xmlrpcwsc is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Lesser General Public License for more details.
/// 
/// You should have received a copy of the GNU Lesser General Public License
/// along with xmlrpcwsc.  If not, see <http://www.gnu.org/licenses/>.
////
using System;
using XmlRpc;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCLAppConfig;

namespace TilesApp.Odoo
{

    /// <summary>
    /// This class, test the xmlrpc component on odoo 9
    /// </summary>
    class OdooConnection {

        private static string Url = ConfigurationManager.AppSettings["SACODOO_URL"], db = ConfigurationManager.AppSettings["SACODOO_DB"], pass = ConfigurationManager.AppSettings["SACODOO_ADMIN_PASSWORD"], user = ConfigurationManager.AppSettings["SACODOO_ADMIN_USER"];
        private static int? adminID;
        private static XmlRpcClient client = new XmlRpcClient();

        private static Dictionary<string,Dictionary<string, object>> cachedUserInfo;
        private static Dictionary<string, object> cachedUsers;
        private static Dictionary<string, Stream> cachedConfigFiles;

        public static void Login()
        {
            client.Url = Url;
            client.Path = "/xmlrpc/2/common";

            try
            {
                XmlRpcRequest requestLogin = new XmlRpcRequest("authenticate");
                requestLogin.AddParams(db, user, pass, XmlRpcParameter.EmptyStruct());
                XmlRpcResponse responseLogin = client.Execute(requestLogin);

                Console.WriteLine("LOGIN: ");
                if (responseLogin.IsFault())
                {
                    throw new Exception(responseLogin.GetFaultString());
                }
                else
                {
                    adminID = responseLogin.GetInt();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public Dictionary<string,object> GetUserInfo(string barcode, bool forceCacheUpdate = false)
        {
            // CHECK IF ALREADY CACHED
            if (cachedUserInfo.ContainsKey(barcode) & !forceCacheUpdate) { return cachedUserInfo[barcode]; }

            // OTHERWISE DO
            List<object> ids = new List<object>();
            List<string> names = new List<string>();
            Dictionary<string, object> userInfo = new Dictionary<string, object>();

            try
            {
                // LOGIN IF NECESSARY
                if(adminID==null){ Login(); };

                // SEARCH
                client.Path = "/xmlrpc/2/object";

                XmlRpcRequest requestSearch = new XmlRpcRequest("execute_kw");
                requestSearch.AddParams(db, adminID, pass, "hr.employee", "search_read",
                    XmlRpcParameter.AsArray(
                        XmlRpcParameter.AsArray(
                            XmlRpcParameter.AsArray("barcode", "=", barcode)
                        )
                    ),
                    XmlRpcParameter.AsStruct(
                        XmlRpcParameter.AsMember("fields", XmlRpcParameter.AsArray("name", "department_id", "job_id", "address_id", "category_ids", "gender", "pin"))
                    )
                );

                XmlRpcResponse responseSearch = client.Execute(requestSearch);

                Console.WriteLine("REQUEST (SEARCH): ");
                client.WriteRequest(Console.Out);
                Console.WriteLine("RESPONSE (SEARCH): ");
                if (responseSearch.IsFault())
                {
                    Console.WriteLine(responseSearch.GetFaultString());
                }
                else
                {
                    Console.WriteLine(responseSearch.GetString());
                    List<object> responseList = (List<object>)responseSearch.GetObject(); //List with one element
                    foreach (object fields in responseList)
                    {
                        Dictionary<string, object> dict = (Dictionary<string, object>)fields;
                        foreach (KeyValuePair<string, object> kv in dict)
                        {
                            Console.WriteLine(kv.Key + " - " + kv.Value.ToString());
                            if (kv.Key == "name" || kv.Key == "id") userInfo.Add(kv.Key, kv.Value.ToString());
                        }
                        ids = (List<object>)dict["category_ids"];
                    }
                }
                if (ids.Count > 0)
                {
                    requestSearch = new XmlRpcRequest("execute_kw");
                    requestSearch.AddParams(db, adminID, pass, "hr.employee.category", "search_read",
                        XmlRpcParameter.AsArray(
                            XmlRpcParameter.AsArray(
                                XmlRpcParameter.AsArray("id", "in", ids)
                            )
                        ),
                        XmlRpcParameter.AsStruct(
                            XmlRpcParameter.AsMember("fields", XmlRpcParameter.AsArray("name"))
                        )
                    );

                    responseSearch = client.Execute(requestSearch);
                    if (responseSearch.IsFault())
                    {
                        Console.WriteLine(responseSearch.GetFaultString());
                    }
                    else
                    {
                        Console.WriteLine(responseSearch.GetString());
                        List<object> responseList = (List<object>)responseSearch.GetObject(); //List with one element
                        foreach (object fields in responseList)
                        {
                            Dictionary<string, object> dict = (Dictionary<string, object>)fields;
                            foreach (KeyValuePair<string, object> kv in dict)
                            {
                                Console.WriteLine(kv.Key + " - " + kv.Value.ToString());
                            }
                            names.Add(dict["name"].ToString());
                        }

                    }
                }
                userInfo.Add("tags", names);
                cachedUserInfo.Add(barcode, userInfo);
                return userInfo;
            }
            catch 
            {
                return null;
            }
            
        }
        public Dictionary<string, object> GetUsers(bool forceCacheUpdate = false)
        {
            // CHECK IF ALREADY CACHED
            if (cachedUsers != null & !forceCacheUpdate) { return cachedUsers; }

            // OTHERWISE DO
            List<string> names = new List<string>();

            Dictionary<string, object> users = new Dictionary<string, object>();
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

                Console.WriteLine("REQUEST (SEARCH): ");
                client.WriteRequest(Console.Out);
                Console.WriteLine("RESPONSE (SEARCH): ");
                if (responseSearch.IsFault())
                {
                    Console.WriteLine(responseSearch.GetFaultString());
                }
                else
                {
                    Console.WriteLine(responseSearch.GetString());
                    List<object> responseList = (List<object>)responseSearch.GetObject(); //List with one element
                    foreach (object fields in responseList)
                    {
                        Dictionary<string, object> dict = (Dictionary<string, object>)fields;
                        tags.Add(dict["id"].ToString(), dict["name"].ToString());
                    }
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

                Console.WriteLine("REQUEST (SEARCH): ");
                client.WriteRequest(Console.Out);
                Console.WriteLine("RESPONSE (SEARCH): ");
                if (responseSearch.IsFault())
                {
                    Console.WriteLine(responseSearch.GetFaultString());
                }
                else
                {
                    Console.WriteLine(responseSearch.GetString());
                    List<object> responseList = (List<object>)responseSearch.GetObject(); //List with one element
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
                cachedUsers = users;
                return users;
            }
            catch (WebServiceException)
            {
                users.Add("error", "internet");
                return users;
            }
            catch (System.InvalidCastException)
            {
                users.Add("error","odoo");
                return users;
            }
        }        

        public void CreateLog()
        {
            XmlRpcClient client = new XmlRpcClient();
            client.Url = Url;
            client.Path = "/xmlrpc/2/common";

            // LOGIN
            XmlRpcRequest requestLogin = new XmlRpcRequest("authenticate");
            requestLogin.AddParams(db, user, pass, XmlRpcParameter.EmptyStruct());

            XmlRpcResponse responseLogin = client.Execute(requestLogin);

            Console.WriteLine("authenticate");
            Console.WriteLine("REQUEST: ");
            client.WriteRequest(Console.Out);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("RESPONSE: ");
            client.WriteResponse(Console.Out);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("LOGIN: ");
            if (responseLogin.IsFault())
            {
                Console.WriteLine(responseLogin.GetFaultString());
            }
            else
            {
                Console.WriteLine(responseLogin.GetString());
            }

            // CREATE

            client.Path = "/xmlrpc/2/object";
            XmlRpcRequest requestCreate = new XmlRpcRequest("execute_kw");
            requestCreate.AddParams(db, responseLogin.GetInt(), pass, "maintenance.request", "create",
                XmlRpcParameter.AsArray(
                    XmlRpcParameter.AsStruct(
                        XmlRpcParameter.AsMember("name", "Phone Log 2")
                      , XmlRpcParameter.AsMember("employee_id", 5)
                      , XmlRpcParameter.AsMember("maintenance_team_id", 3)
                      , XmlRpcParameter.AsMember("description", "This is a test")
                      //, XmlRpcParameter.AsMember("priority", 2)
                      , XmlRpcParameter.AsMember("email_cc", "lalala@email.com")
                    )
                )
            );

            XmlRpcResponse responseCreate = client.Execute(requestCreate);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("create");
            Console.WriteLine("REQUEST: ");
            client.WriteRequest(Console.Out);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("RESPONSE: ");
            client.WriteResponse(Console.Out);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("READ: ");
            if (responseCreate.IsFault())
            {
                Console.WriteLine(responseCreate.GetFaultString());
            }
            else
            {
                Console.WriteLine(responseCreate.GetString());
            }
        }

    }
}
