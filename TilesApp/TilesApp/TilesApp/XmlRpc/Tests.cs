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

namespace XmlRpc {

    /// <summary>
    /// This class, test the xmlrpc component on odoo 9
    /// </summary>
    class TestClass {

        //public static string Url = "http://localhost:8069/xmlrpc/2", db = "odoo9", pass = "admin", user = "admin";
        public static string ImgPath = "D:\\Users\\cbonillo\\source\\repos\\TilesApp\\TilesApp\\TilesApp\\TilesApp\\";
        public static string Url = "https://sacodoo-13-0-parametric-626490.dev.odoo.com", db = "sacodoo-13-0-parametric-626490", pass = "Saco-2019", user = "miguelfontgivell@saco.com";

        public void TestRequestXml() {
            XmlRpcRequest request = new XmlRpcRequest("version");
            request.AddParam(false);
            request.AddParam(3);
            request.AddParam(4.9);
            request.AddParam(DateTime.Now);
            request.AddParam(DateTime.UtcNow);
            request.AddParam(Encoding.UTF8.GetBytes("hello"));

            Dictionary<string, object> dictest = new Dictionary<string, object>();
            dictest.Add("hello", "hello");
            // request.AddParam(dictest);

            List<object> listtest = new List<object>();
            listtest.Add(3);
            listtest.Add("hello");
            listtest.Add(dictest);
            request.AddParam(listtest);

            XmlDocument xmlRequest = RequestFactory.BuildRequest(request);

            xmlRequest.Save(Console.Out);

            XmlRpcClient client = new XmlRpcClient();
            client.AppName = "Test";
            Console.WriteLine("\n");
            Console.WriteLine(client.GetUserAgent());
        }

        public void TestReadVersion() {
            XmlRpcClient client = new XmlRpcClient();
            client.Url = Url;
            client.Path = "/xmlrpc/2/common";

            XmlRpcResponse response = client.Execute("version");

            Console.WriteLine("version");
            Console.WriteLine("REQUEST: ");
            client.WriteRequest(Console.Out);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("RESPONSE: ");
            client.WriteResponse(Console.Out);

            Console.WriteLine();
            Console.WriteLine();
            if (response.IsFault()) {
                Console.WriteLine(response.GetFaultString());
            } else {
                Console.WriteLine(response.GetString());
            }
        }

        public void TestReadRecords() {
            XmlRpcClient client = new XmlRpcClient();
            client.Url = Url;
            client.Path = "/xmlrpc/2/common";           

            // LOGIN
            XmlRpcRequest requestLogin = new XmlRpcRequest("authenticate");
            requestLogin.AddParams(db, user, pass, XmlRpcParameter.EmptyStruct());

            XmlRpcResponse responseLogin = client.Execute(requestLogin);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("LOGIN: ");
            if (responseLogin.IsFault()) {
                Console.WriteLine(responseLogin.GetFaultString());
            } else {
                Console.WriteLine(responseLogin.GetString());
            }

            // SEARCH
            client.Path = "/xmlrpc/2/object";

            XmlRpcRequest requestSearch = new XmlRpcRequest("execute_kw");
            requestSearch.AddParams(db, responseLogin.GetInt(), pass, "res.partner", "search", 
                XmlRpcParameter.AsArray(
                    XmlRpcParameter.AsArray(
                        XmlRpcParameter.AsArray("is_company", "=", true), XmlRpcParameter.AsArray("customer", "=", true)
                    )
                )
            );

            requestSearch.AddParamStruct(
                XmlRpcParameter.AsMember("limit", 2)
            );

            XmlRpcResponse responseSearch = client.Execute(requestSearch);
            
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("SEARCH: ");
            if (responseSearch.IsFault()) {
                Console.WriteLine(responseSearch.GetFaultString());
            } else {
                Console.WriteLine(responseSearch.GetString());
            }

            // READ
            XmlRpcRequest requestRead = new XmlRpcRequest("execute_kw");
            requestRead.AddParams(db, responseLogin.GetInt(), pass, "res.partner", "read",                                           
                XmlRpcParameter.AsArray(
                    responseSearch.GetArray()
                )
            );

            requestRead.AddParamStruct(XmlRpcParameter.AsMember("fields", 
                    XmlRpcParameter.AsArray("name")
                )
            );

            XmlRpcResponse responseRead = client.Execute(requestRead);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("READ: ");
            if (responseRead.IsFault()) {
                Console.WriteLine(responseRead.GetFaultString());
            } else {
                Console.WriteLine(responseRead.GetString());
            }
        }

        public void TestResponseXml() {
            XmlDocument testDoc = new XmlDocument();
            // testDoc.AppendChild(testDoc.CreateElement("methodResponse"));
            // testDoc.LoadXml("<methodResponse><fault><value><struct><member><name>faultCode</name><value><int>1</int></value></member><member><name>faultString</name><value><string>Error</string></value></member></struct></value></fault></methodResponse>");
            testDoc.LoadXml("<methodResponse><params><param><value><array><data><value><int>7</int></value><value><int>11</int></value><value><int>8</int></value><value><int>44</int></value><value><int>10</int></value><value><int>12</int></value></data></array></value></param></params></methodResponse>");

            testDoc.Save(Console.Out);
            XmlRpcResponse response = ResponseFactory.BuildResponse(testDoc);

            if (response.IsFault()) {
                Console.WriteLine(response.GetFaultString());
            } else {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(response.GetString());
            }
        }     

        public void TestSearchReadRecords(string condition) {
            XmlRpcClient client = new XmlRpcClient();
            client.Url = Url;
            client.Path = "/xmlrpc/2/common";           

            // LOGIN
            XmlRpcRequest requestLogin = new XmlRpcRequest("authenticate");
            requestLogin.AddParams(db, user, pass, XmlRpcParameter.EmptyStruct());
            XmlRpcResponse responseLogin = client.Execute(requestLogin);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("LOGIN: ");
            if (responseLogin.IsFault()) {
                Console.WriteLine(responseLogin.GetFaultString());
            } else {
                Console.WriteLine(responseLogin.GetString());
            }

            // SEARCH
            client.Path = "/xmlrpc/2/object";

            //XmlRpcRequest requestSearch = new XmlRpcRequest("execute_kw");
            //requestSearch.AddParams(db, responseLogin.GetInt(), pass, "res.partner", "search_read",
            //    XmlRpcParameter.AsArray(
            //        XmlRpcParameter.AsArray(
            //            // XmlRpcParameter.AsArray("is_company", "=", true), XmlRpcRequest.AsArray("customer", "=", true)
            //            XmlRpcParameter.AsArray("name", "ilike", condition)
            //        )
            //    ),
            //    XmlRpcParameter.AsStruct(
            //        XmlRpcParameter.AsMember("fields", XmlRpcParameter.AsArray("name", "email"))
            //    // ,XmlRpcParameter.AsMember("limit", 2)
            //    )
            //);
            //XmlRpcRequest requestSearch = new XmlRpcRequest("execute_kw");
            //requestSearch.AddParams(db, responseLogin.GetInt(), pass, "mrp.workcenter", "search_read",
            //    XmlRpcParameter.AsArray(
            //        XmlRpcParameter.AsArray(
            //            // XmlRpcParameter.AsArray("is_company", "=", true), XmlRpcRequest.AsArray("customer", "=", true)
            //            XmlRpcParameter.AsArray("name", "ilike", "t")
            //        )
            //    ),
            //    XmlRpcParameter.AsStruct(
            //        XmlRpcParameter.AsMember("fields", XmlRpcParameter.AsArray("name", "code"))
            //    // ,XmlRpcParameter.AsMember("limit", 2)
            //    )
            //);
            XmlRpcRequest requestSearch = new XmlRpcRequest("execute_kw");
            requestSearch.AddParams(db, responseLogin.GetInt(), pass, "product.template", "search_read",
                XmlRpcParameter.AsArray(
                    XmlRpcParameter.AsArray(
                        // XmlRpcParameter.AsArray("is_company", "=", true), XmlRpcRequest.AsArray("customer", "=", true)
                        XmlRpcParameter.AsArray("name", "ilike", condition)
                    )
                ),
                XmlRpcParameter.AsStruct(
                    XmlRpcParameter.AsMember("fields", XmlRpcParameter.AsArray("name", "bom_count"))
                // ,XmlRpcParameter.AsMember("limit", 2)
                )
            );

            XmlRpcResponse responseSearch = client.Execute(requestSearch);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("REQUEST (SEARCH): ");
            client.WriteRequest(Console.Out);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("RESPONSE (SEARCH): ");
            if (responseSearch.IsFault()) {
                Console.WriteLine(responseSearch.GetFaultString());
            } else {
                Console.WriteLine(responseSearch.GetString());
            }
           
        }

        public void TestCreateRecord() {
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
            if (responseLogin.IsFault()) {
                Console.WriteLine(responseLogin.GetFaultString());
            } else {
                Console.WriteLine(responseLogin.GetString());
            }

            // CREATE

            client.Path = "/xmlrpc/2/object";

            //XmlRpcRequest requestCreate = new XmlRpcRequest("execute_kw");
            //requestCreate.AddParams(db, responseLogin.GetInt(), pass, "res.partner", "create",                                           
            //    XmlRpcParameter.AsArray(
            //        XmlRpcParameter.AsStruct(
            //            XmlRpcParameter.AsMember("name", "Lauren Bacall")
            //            , XmlRpcParameter.AsMember("image_1920", Convert.ToBase64String(File.ReadAllBytes("img/lauren.jpg")))
            //            , XmlRpcParameter.AsMember("email", "lauren.bacall@saco.com")
            //        )
            //    )
            //);

            //XmlRpcRequest requestCreate = new XmlRpcRequest("execute_kw");
            //requestCreate.AddParams(db, responseLogin.GetInt(), pass, "project.project", "create",
            //    XmlRpcParameter.AsArray(
            //        XmlRpcParameter.AsStruct(
            //            XmlRpcParameter.AsMember("name", "ProjectFromCodeEmail6")
            //          , XmlRpcParameter.AsMember("alias_name", "project")
            //          , XmlRpcParameter.AsMember("alias_domain", "sacodoo-13-0-parametric-626490.dev.odoo.com")
            //        )
            //    )
            //);

            XmlRpcRequest requestCreate = new XmlRpcRequest("execute_kw");
            requestCreate.AddParams(db, responseLogin.GetInt(), pass, "mrp.workcenter", "create",
                XmlRpcParameter.AsArray(
                    XmlRpcParameter.AsStruct(
                        XmlRpcParameter.AsMember("name", "WOTest")
                      , XmlRpcParameter.AsMember("code", "codeTest")
                      , XmlRpcParameter.AsMember("costs_hour", 45)
                      , XmlRpcParameter.AsMember("time_efficiency", 78)
                      , XmlRpcParameter.AsMember("capacity", 3)
                      , XmlRpcParameter.AsMember("oee_target", 58)
                      , XmlRpcParameter.AsMember("time_start", 100)
                      , XmlRpcParameter.AsMember("time_stop", 250)
                      , XmlRpcParameter.AsMember("nore", "This is a note")
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
            if (responseCreate.IsFault()) {
                Console.WriteLine(responseCreate.GetFaultString());
            } else {
                Console.WriteLine(responseCreate.GetString());
            }
        }

        public void Main(string[] args) {
            //TestRequestXml();
            //TestResponseXml();
            //TestReadVersion();
            //TestReadRecords();
            //TestCreateRecord();
            TestSearchReadRecords("t");
            Console.ReadKey();
        }
    }
}
