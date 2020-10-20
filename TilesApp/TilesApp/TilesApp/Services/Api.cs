using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TilesApp.Models.DataModels;
using Xamarin.Forms;
using static System.Environment;

namespace TilesApp.Services
{
    public static class Api
    {

        private static string BlackBoxesUri = "https://blackboxestest.azurewebsites.net/"; //TEST
        //private static string BlackBoxesUri = "https://blackboxes.azurewebsites.net/";

        public async static Task<string> GetProjectsList()
        {
            string result = "";
            try
            {
                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.User.OBOToken);
                    string Uri = BlackBoxesUri + "_projects/__index";
                    HttpResponseMessage response = await client.GetAsync(Uri);
                    if (response.IsSuccessStatusCode)
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
        
        public async static Task<string> GetFamiliesList()
        {
            string result = "";
            try
            {
                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.User.OBOToken);
                    string Uri = BlackBoxesUri + App.CurrentProjectSlug + "/r/_forms/__index";
                    HttpResponseMessage response = await client.GetAsync(Uri);
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

        public async static Task<string> GetFieldsList(string slug)
        {
            string result = "";
            try
            {
                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.User.OBOToken);
                    string Uri = BlackBoxesUri + App.CurrentProjectSlug + "/r/" + slug + "/__show";
                    HttpResponseMessage response = await client.GetAsync(Uri);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception e)
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "Error", e.Message);
            }
            return result;
        }

        public async static Task<string> GetPhases()
        {
            string result = "";
            try
            {
                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.User.OBOToken);
                    string Uri = BlackBoxesUri + "_api/__getPhases";
                    HttpResponseMessage response = await client.GetAsync(Uri);
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

        public async static Task<string> GetPrimitiveTypes()
        {
            string result = "";
            try
            {
                if (App.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.User.OBOToken);
                    string Uri = BlackBoxesUri + "_api/__getPrimitiveTypes";
                    HttpResponseMessage response = await client.GetAsync(Uri);
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

        public async static Task<string> PostFormContent(string slug, string protofamily_id, Dictionary<string,object> form_info)
        {
            string result = "";
            try
            {
                if (App.IsConnected)
                {

                    HttpClient client = new HttpClient();
                    string Uri = BlackBoxesUri + App.CurrentProjectSlug + "/e/_elements/__add";
                    
                    string contentRequest = String.Format(CultureInfo.InvariantCulture, Uri);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, contentRequest);
                    
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", App.User.OBOToken);
                    
                    // Data to send 
                    List<object> payload_list = new List<object>();

                    ///// TEST
                    //var payload_content = new Dictionary<string, object>(){
                    //    {"k11", "v11"},
                    //    {"k12", "v12"},
                    //};
                    //payload_list.Add(payload_content);
                    //payload_content = new Dictionary<string, object>(){
                    //    {"k21", "v21"},
                    //    {"k22", "v22"},
                    //};
                    //payload_list.Add(payload_content);

                    payload_list.Add(form_info);
                    
                    var content = new Dictionary<string, object>(){
                        {"protofamily_id", protofamily_id},
                        {"payload", payload_list},
                    };

                    string ouputdata = JsonConvert.SerializeObject(content);
                    request.Content = new StringContent(ouputdata, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.SendAsync(request).Result;

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

        public static List<Web_Field> FormatFields(string jsonFields)
        {

            List<Web_Field> fieldList = new List<Web_Field>();

            try
            {
                Dictionary<string, object> keyField = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonFields);

                JArray ja = (JArray)keyField["protofamilyfields"];
                List<Dictionary<string, object>> fieldDataList = ja.ToObject<List<Dictionary<string, object>>>();

                foreach (Dictionary<string, object> fieldData in fieldDataList)
                {
                    Web_Field field = new Web_Field();
                    if (fieldData["value_regex"] != null) field.ValueRegEx = fieldData["value_regex"].ToString();
                    if (fieldData["default"] != null) field.Default = fieldData["default"].ToString();
                    if (fieldData["entity_id"] != null) field.EntityId = fieldData["entity_id"].ToString();
                    if (fieldData["ui_index"] != null) field.UIindex = Convert.ToInt32(fieldData["ui_index"]);
                    if (fieldData["type"] != null) field.Type = Convert.ToInt32(fieldData["type"]);
                    if (fieldData["name"] != null) field.Name = fieldData["name"].ToString();
                    if (fieldData["long_name"] != null) field.LongName = fieldData["long_name"].ToString();
                    if (fieldData["description"] != null) field.Description = fieldData["description"].ToString();
                    if (fieldData["slug"] != null) field.Slug = fieldData["slug"].ToString();
                    if (fieldData["primitive_type"] != null)
                    {
                        field.PrimitiveType = Convert.ToInt32(fieldData["primitive_type"]);
                        PrimitiveType p = App.PrimitiveTypes[field.PrimitiveType.ToString()];
                        field.PrimitiveQuantity = p.Length;
                    }
                    if (fieldData["field_is_required"] != null) field.ValueIsRequired = (bool)fieldData["field_is_required"];
                    if (fieldData.ContainsKey("values_are_unique")) field.ValueIsUnique = (bool)fieldData["values_are_unique"];
                    if (fieldData.ContainsKey("value_is_unique")) field.ValueIsUnique = (bool)fieldData["value_is_unique"];
                    if (fieldData["canWrite"] != null) field.CanWrite = (bool)fieldData["canWrite"];
                    if (fieldData["canRead"] != null) field.CanRead = (bool)fieldData["canRead"];
                    fieldList.Add(field);
                }
            }
            catch {}
            return fieldList;
        }
    }
}
